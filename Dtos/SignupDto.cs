using System.ComponentModel.DataAnnotations;
namespace SLink.Dtos;
public class SignupDto
{
    [Required]
    public string Username{get;set;}
    [Required]
    public string Email{get;set;}
    [Required]
    public string Password{get;set;}
    
}