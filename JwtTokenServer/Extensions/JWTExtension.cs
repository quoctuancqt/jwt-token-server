namespace JwtTokenServer.Extensions
{
    using JwtTokenServer.Middlewares;
    using JwtTokenServer.Models;
    using JwtTokenServer.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;

    public static class JWTExtension
    {
        public static IApplicationBuilder JWTBearerToken(this IApplicationBuilder app,
            IConfiguration configuration, TokenProviderOptions tokenProviderOptions = null)
        {
            var tokenProviderOptionsOpt = tokenProviderOptions ?? new TokenProviderOptions
            {
                Path = JwtSettings.DefaultPath,
                SecurityKey = JwtSettings.DefaultSecretKey,
                Expiration = TimeSpan.FromMinutes(+1440)
            };

            app.UseMiddleware<TokenProviderMiddleware>(tokenProviderOptionsOpt);

            return app;
        }

        public static IServiceCollection JWTAddAuthentication(this IServiceCollection services,
            IConfiguration configuration, TokenValidationParameters tokenValidationParameters = null,
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            var tokenValidationParametersOpt = tokenValidationParameters ?? new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(JwtSettings.DefaultSecretKey))
            };

            services.AddAuthentication(defaultScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParametersOpt;
            });

            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }

        public static IServiceCollection JWTAddAuthentication(this IServiceCollection services,
            Action<JwtBearerOptions> jwtBearerOptions = null, 
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            services.AddAuthentication(defaultScheme).AddJwtBearer(jwtBearerOptions);

            return services;
        }

        public static IServiceCollection AddAccountManager<TAccountManager>(this IServiceCollection services)
            where TAccountManager : class, IAccountManager
        {
            services.AddScoped<IAccountManager, TAccountManager>();

            return services;
        }
    }
}
