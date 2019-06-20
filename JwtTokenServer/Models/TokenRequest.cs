namespace JwtTokenServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    public class TokenRequest
    {
        public TokenRequest() { }

        public TokenRequest(TokenProviderOptions options, List<Claim> claims)
        {
            Claims = claims;
            Expiration = options.Expiration;
            SecurityKey = options.SecurityKey;
            Responses = new Dictionary<string, object>();
            Issuer = options.Issuer;
            Audience = options.Audience;
        }

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public List<Claim> Claims { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(+30);
        public double TotalSeconds => Expiration.TotalSeconds;
        public string SecurityKey { get; set; } = JwtSettings.DefaultSecretKey;
        public IDictionary<string, object> Responses { get; set; }
    }
}
