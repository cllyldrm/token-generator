using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TokenGenerator.Managers.Interfaces;
using TokenGenerator.Models;
using ClaimValueTypes = TokenGenerator.Constants.ClaimValueTypes;

namespace TokenGenerator.Managers
{
    public class TokenManager : ITokenManager
    {
        private readonly IOptions<TokenSettings> _tokenSettings;

        public TokenManager(IOptions<TokenSettings> tokenSettings)
        {
            _tokenSettings = tokenSettings;
        }

        public string GenerateToken(Dictionary<string, object> claims)
        {
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                                                                                 issuer: _tokenSettings.Value.Issuer,
                                                                                 audience: _tokenSettings.Value.Audience,
                                                                                 claims: SetClaims(claims),
                                                                                 expires: DateTime.Now.AddMinutes(_tokenSettings.Value.ExpireMinute),
                                                                                 signingCredentials: new SigningCredentials(
                                                                                                                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Value.SigningKey)),
                                                                                                                            SecurityAlgorithms.HmacSha256)
                                                                                ));
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public Dictionary<string, object> GetClaims(string token)
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token,
                                                                        new TokenValidationParameters
                                                                        {
                                                                            ValidateAudience = false,
                                                                            ValidateIssuer = false,
                                                                            ValidateIssuerSigningKey = true,
                                                                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Value.SigningKey)),
                                                                            ValidateLifetime = false
                                                                        },
                                                                        out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(principal.Claims.First(claim => claim.Type == ClaimValueTypes.Json).Value);
        }

        #region private functions

        private static IEnumerable<Claim> SetClaims(Dictionary<string, object> userClaims)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimValueTypes.Json, JsonConvert.SerializeObject(userClaims), ClaimValueTypes.Json)
            };
        }

        #endregion
    }
}