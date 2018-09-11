﻿namespace JwtTokenServer.Models
{
    using System;

    public class TokenProviderOptions
    {
        public TokenProviderOptions() { }

        public string Path { get; set; } = JwtSettings.DefaultPath;

        public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(+1);

        public string SecurityKey { get; set; } = JwtSettings.DefaultSecretKey;
    }
}