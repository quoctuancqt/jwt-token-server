using JwtTokenServer.Middlewares;
using JwtTokenServer.Models;
using JwtTokenServer.Proxies;
using JwtTokenServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace JwtTokenServer.Extensions
{
    public static class JWTExtension
    {
        public static IApplicationBuilder UseJWTBearerTokenMiddleware(this IApplicationBuilder app,
            IConfiguration configuration, TokenProviderOptions tokenProviderOptions = null)
        {
            var jwtOptions = configuration.Bind<JwtOptions>(JwtOptions.Name);

            string secretKey = string.IsNullOrEmpty(jwtOptions.SecurityKey) ? JwtSettings.DefaultSecretKey : jwtOptions.SecurityKey;

            var tokenProviderOptionsOpt = tokenProviderOptions ?? new TokenProviderOptions
            {
                Path = JwtSettings.DefaultPath,
                SecurityKey = secretKey,
                Expiration = TimeSpan.FromMinutes(+1440),
                Audience = jwtOptions.Audience,
                Issuer = jwtOptions.Issuer
            };

            app.UseMiddleware<TokenProviderMiddleware>(tokenProviderOptionsOpt);

            return app;
        }

        public static IServiceCollection AddJWTBearerToken(this IServiceCollection services, IConfiguration configuration,
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme,
            Action<AuthenticationOptions> authenticationOptions = null,
            TokenValidationParameters tokenValidationParameters = null,
            Action<JwtBearerOptions> jwtBearerOptions = null)
        {
            services.AddJwtTokenServices(configuration);

            var jwtOptions = configuration.Bind<JwtOptions>(JwtOptions.Name);

            string secretKey = string.IsNullOrEmpty(jwtOptions.SecurityKey) ? JwtSettings.DefaultSecretKey : jwtOptions.SecurityKey;

            AuthenticationBuilder builder = default;

            if (authenticationOptions == null)
            {
                builder = services.AddAuthentication(defaultScheme);
            }
            else
            {
                builder = services.AddAuthentication(authenticationOptions);
            }

            builder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters ?? new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

                jwtBearerOptions?.Invoke(options);
            });

            return services;
        }

        public static IServiceCollection AddJwtTokenServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.Name));

            var jwtOptions = configuration.Bind<JwtOptions>(JwtOptions.Name);

            services.AddHttpClient<OAuthClient>(typeof(OAuthClient).Name, client => client.BaseAddress = new Uri(jwtOptions.Issuer));

            services.Scan(scan =>
            {
                scan.FromEntryAssembly()
                    .AddClasses(classes => classes.AssignableTo<IAccountManager>())
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }
    }
}
