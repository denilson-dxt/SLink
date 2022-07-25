using SLink.Data;

namespace SLink.Models;
public class ShortLink
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string OriginalLink { get; set; }
    public string? FullShortedLink { get; set; }

    public string? Key { get; set; }
    public bool IsProtected { get; set; } = false;
    public ApplicationUser Owner { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateAt { get; set; }
}