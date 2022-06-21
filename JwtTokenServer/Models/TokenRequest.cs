using System;
using System.Collections.Generic;

namespace JwtTokenServer.Models
{
    public class TokenRequest
    {
        public TokenRequest() { }

        public TokenRequest(TokenProviderOptions options, List<CustomClaim> claims)
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
        public List<CustomClaim> Claims { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(+30);
        public double TotalSeconds => Expiration.TotalSeconds;
        public string SecurityKey { get; set; } = JwtSettings.DefaultSecretKey;
        public IDictionary<string, object> Responses { get; set; }
    }

    public class CustomClaim
    {
        public CustomClaim(string type, object value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; }
        public object Value { get; set; }
    }
}
