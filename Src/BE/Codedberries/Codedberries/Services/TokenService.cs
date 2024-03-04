using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Codedberries.Services
{
    public static class TokenService
    {
        private const string SecretKey = "secret-keysecret-keysecret-keysecret-keysecret-keysecret-keysecret-keysecret-keysecret-keysecret-key";
        private static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        private static readonly SigningCredentials Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

        public static string GenerateToken(string email)
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

        public static bool ValidateToken(string token)
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
    }
}
