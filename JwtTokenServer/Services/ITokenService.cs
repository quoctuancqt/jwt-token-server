namespace JwtTokenServer.Services
{
    using JwtTokenServer.Models;
    using System.Collections.Generic;

    public interface ITokenService
    {
        IDictionary<string, object> GenerateToken(TokenRequest dto);

        IDictionary<string, object> RefreshAccessToken(string token);

        void RevokeRefreshToken(string token);

    }
}
