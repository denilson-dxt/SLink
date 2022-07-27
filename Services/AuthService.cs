using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.Extensions.Options;
using SLink.Data;
using SLink.Configuration;
using SLink.Dtos;
namespace SLink.Services;
public class AuthService
{
    private readonly JwtBearerTokenSettings _jwtBearerTOkenSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    public AuthService(IOptions<JwtBearerTokenSettings> jwtBearerTOkenSettings, UserManager<ApplicationUser> userManager)
    {
        _jwtBearerTOkenSettings = jwtBearerTOkenSettings.Value;
        _userManager = userManager;
    }

    public async Task<object> AuthenticateAsync(LoginDto credentials)
    {
        var user = await ValidateUser(credentials);
        if (user == null) return new { Status = "ERROR", Error = "Wrong password or Ivalid User" };
        var token = GenerateToken(user);
        return new { Status = "OK", User = new UserDto {Id = user.Id, Username = user.UserName, Email = user.Email, EmailConfirmed = user.EmailConfirmed }, Token = token };
    }

    private async Task<ApplicationUser> ValidateUser(LoginDto credentials)
    {
        var user = await _userManager.FindByNameAsync(credentials.Username);
        if (user == null)
            return null;
        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, credentials.Password);
        return result == PasswordVerificationResult.Success ? user : null;
    }

    private object GenerateToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtBearerTOkenSettings.SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName.ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToString())
            }),
            Expires = DateTime.UtcNow.AddSeconds(_jwtBearerTOkenSettings.ExpiryTimeInSeconds),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = _jwtBearerTOkenSettings.Audience,
            Issuer = _jwtBearerTOkenSettings.Issuer
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}