using Codedberries.Environment;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Codedberries.Services
{
    public class TokenService
    {
        private readonly Config _config;

        private readonly string SecretKey;
        private readonly SymmetricSecurityKey SecurityKey;
        private readonly SigningCredentials Credentials;

        public TokenService(IOptions<Config> config)
        {
            _config = config.Value;

            SecretKey = _config.SecretKey;
            SecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
            Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        }

        public string GenerateToken(string email)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, email)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: Credentials
            );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var claims = handler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = SecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetUsernameFromToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var emailClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            return emailClaim?.Value;
        }
    }
}
