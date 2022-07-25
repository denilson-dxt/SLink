using System.ComponentModel.DataAnnotations;
using SLink.Data;

public class ShortLinkDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    [Required]
    public string OriginalLink { get; set; }
    public string? FullShortedLink { get; set; }
    public string? Key { get; set; }
    public bool IsProtected { get; set; } = false;
    public ApplicationUser? Owner { get; set; }
    public bool? IsActive { get; set; } = true;

}