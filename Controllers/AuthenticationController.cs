using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using SLink.Dtos;
using SLink.Data;
using SLink.Services;

namespace SLink.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthService _authService;
    public AuthenticationController(UserManager<ApplicationUser> userManager, AuthService authService)
    {
        _userManager = userManager;
        _authService = authService;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register(SignupDto data)
    {
        var user = new ApplicationUser
        {
            UserName = data.Username,
            Email = data.Email,
            PhoneNumberConfirmed = true,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, data.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new {Status = "ERROR", Errors = result.Errors});
        }
        return Ok(new
        {
            Status = "OK",
            User = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            }
        });
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login(LoginDto data)
    {
        var result = await _authService.AuthenticateAsync(data);
        return Ok(result);
    }

    [HttpGet]
    [Route("Check-Login")]
    [Authorize]
    public async Task<ActionResult<IdentityUser>> CheckLogin()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        return Ok(new {Status = "OK", User = new UserDto {Id = user.Id, Username = user.UserName, Email = user.Email, EmailConfirmed = user.EmailConfirmed} });
    }

}