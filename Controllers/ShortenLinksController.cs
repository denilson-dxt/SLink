using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SLink.Data;
using SLink.Services;


namespace SLink.Controllers;

[ApiController]
[Route("api/links")]
public class ShortenLinksController : ControllerBase

{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ShortenLinkService _shortenLinkService;
    public ShortenLinksController(UserManager<ApplicationUser> userManager, ShortenLinkService shortenLinkService)
    {
        _userManager = userManager;
        _shortenLinkService = shortenLinkService;
    }

    [HttpGet]
    [Route("get-link")]
    public async Task<ActionResult> GetLinkByCode(string code, string? key)
    {
        System.Console.WriteLine(User.Identity.IsAuthenticated);
        ApplicationUser user = null;
        if (User.Identity.IsAuthenticated)
            user = await _userManager.FindByNameAsync(User.Identity.Name);
        var result = await _shortenLinkService.GetByCode(code, key, user);
        System.Console.WriteLine(result);
        if (result == null)
            return BadRequest(new { Status = "Error", Message = "Invalid link or invalid key provided" });
        return Ok(result);
    }
}