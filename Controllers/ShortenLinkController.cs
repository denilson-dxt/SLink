using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using SLink.Data;
using SLink.Models;
using SLink.Services;
namespace SLink.Controllers;

[ApiController]
[Authorize]
[Route("api/shorten-link")]
public class ShortenLinkController : ControllerBase
{
    private readonly ShortenLinkService _shortenLinkService;
    private readonly UserManager<ApplicationUser> _userManager;
    public ShortenLinkController(ShortenLinkService shortenLinkService, UserManager<ApplicationUser> userManager)
    {
        _shortenLinkService = shortenLinkService;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<ShortLinkDto>> CreateShortLink(ShortLinkDto data)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var result = await _shortenLinkService.CreateAsync(data, user);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-user-short-links")]
    public async Task<ActionResult<List<ShortLink>>> GetUserShortLinks()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var links = await _shortenLinkService.GetByUserAsync(user);
        return Ok(links);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("get")]
    public async Task<ActionResult> GetShortLink(string code, string? key = "")
    {
        System.Console.WriteLine(User.Identity.IsAuthenticated);
        ApplicationUser user = null;
        if (User.Identity.IsAuthenticated)
            user = await _userManager.FindByNameAsync(User.Identity.Name);
        var result = await _shortenLinkService.GetByCode(code, key, user);
        if (result == null)
            return BadRequest(new { Status = "Error", Message = "Invalid link or invalid key provided" });
        return Ok(result);
    }


    [Authorize]
    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> UpdateShortLink(ShortLinkDto data)
    {
        var result = await _shortenLinkService.UpdateAsync(data);
        if (result == null) return BadRequest(new { Status = "ERROR", Message = "No shortLink found" });
        return Ok(result);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> DeleteShortLink(string code)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
        var result = await _shortenLinkService.DeleteAsync(code, user);
        return Ok(result);
    }
}