using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Redit_api.Models;

namespace Redit_api.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }

    public class TokenService : ITokenService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly SymmetricSecurityKey _key;
        private readonly int _expiresMinutes;

        public TokenService(IConfiguration config)
        {
            _issuer = config["JWT:Issuer"]!;
            _audience = config["JWT:Audience"]!;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
            _expiresMinutes = int.TryParse(config["JWT:ExpiresMinutes"], out var m) ? m : 60;
        }

        public string CreateToken(User user)
        {
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("name", user.Name ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}