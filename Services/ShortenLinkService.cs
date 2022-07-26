using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SLink.Data;
using SLink.Models;


namespace SLink.Services;
public class ShortenLinkService
{
    private readonly ApplicationDbContext _context;
    public ShortenLinkService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ShortLinkDto> CreateAsync(ShortLinkDto data, ApplicationUser user)
    {

        var shortLink = new ShortLink()
        {
            OriginalLink = data.OriginalLink,
            Key = data.Key,
            IsProtected = data.IsProtected,
            Owner = user
        };
        shortLink.Code = await GenerateShortLinkCode();
        shortLink.CreateAt = DateTime.Now;
        shortLink.FullShortedLink = "";
        _context.ShortLinks.Add(shortLink);
        await _context.SaveChangesAsync();

        data.Code = shortLink.Code;
        data.Id = shortLink.Id;

        return data;
    }

    public async Task<List<ShortLink>> GetByUserAsync(ApplicationUser user)
    {
        var links = await _context.ShortLinks.Where(s => s.Owner.Id == user.Id).ToListAsync();
        return links;
    }
    public async Task<ShortLink?> GetByCode(string code, string key, ApplicationUser user)
    {
        var shortLink = await _context.ShortLinks.Where(s => s.Code == code).FirstOrDefaultAsync();

        // Validate key checking the key and the owner
        bool valid = false;
        if (user == null && shortLink.Key == key)
            valid = true;
        if (user != null && shortLink.Owner.Id == user.Id)
            valid = true;

        return valid ? shortLink : null;

    }

    public async Task<ShortLink> UpdateAsync(ShortLinkDto data)
    {
        var shortLink = await _context.ShortLinks.FirstOrDefaultAsync(s => s.Id == data.Id);
        if(shortLink == null)
            return null;
        
        shortLink.IsActive = (bool)data.IsActive;
        shortLink.IsProtected = data.IsProtected;
        shortLink.Key = data.Key;
        shortLink.OriginalLink = data.OriginalLink;
        await _context.SaveChangesAsync();
        return shortLink;
    }
    public async Task<ShortLink> DeleteAsync(string code, ApplicationUser user)
    {
        var shortLink = await _context.ShortLinks.Where(s => s.Code == code && s.Owner.Id == user.Id).FirstOrDefaultAsync();
        _context.ShortLinks.Remove(shortLink);
        await _context.SaveChangesAsync();
        return shortLink;
    }

    private async Task<string> GenerateShortLinkCode()
    {
        string code = Guid.NewGuid().ToString()[0..5];
        while (await _context.ShortLinks.AnyAsync(s => s.Code == code))
        {
            code = Guid.NewGuid().ToString()[0..5];
        }
        return code;
    }
}