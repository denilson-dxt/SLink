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
        shortLink.Code = Guid.NewGuid().ToString()[0..4];
        _context.ShortLinks.Add(shortLink);
        await _context.SaveChangesAsync();
        return data;
    }
    public async Task<List<ShortLink>> GetByUserAsync(ApplicationUser user)
    {
        var links = await _context.ShortLinks.Where(s => s.Owner.Id == user.Id).ToListAsync();
        return links;
    }
    public async Task<ShortLink> GetByCode(string code)
    {
        var shortLink = await _context.ShortLinks.Where(s => s.Code == code).FirstOrDefaultAsync();
        return shortLink;
    }
    public async Task<ShortLink> DeleteAsync(string code)
    {
        var shortLink = await _context.ShortLinks.Where(s => s.Code == code).FirstOrDefaultAsync();
        _context.ShortLinks.Remove(shortLink);
        await _context.SaveChangesAsync();
        return shortLink;
    }
}