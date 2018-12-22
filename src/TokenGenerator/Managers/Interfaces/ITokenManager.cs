using System.Collections.Generic;

namespace TokenGenerator.Managers.Interfaces
{
    public interface ITokenManager
    {
        string GenerateToken(Dictionary<string, object> userClaims);

        string GenerateRefreshToken();

        Dictionary<string, object> GetClaims(string token);
    }
}