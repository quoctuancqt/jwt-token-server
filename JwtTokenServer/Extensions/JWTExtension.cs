namespace JwtTokenServer.Extensions
{
    using JwtTokenServer.Middlewares;
    using JwtTokenServer.Models;
    using JwtTokenServer.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;

    public static class JWTExtension
    {
        private static TokenValidationParameters defaultOptions = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(JwtSettings.DefaultSecretKey))
        };

        public static IApplicationBuilder JWTBearerToken(this IApplicationBuilder app,
            IConfiguration configuration, TokenProviderOptions tokenProviderOptions = null)
        {
            var tokenProviderOptionsOpt = tokenProviderOptions ?? new TokenProviderOptions
            {
                Path = JwtSettings.DefaultPath,
                SecurityKey = JwtSettings.DefaultSecretKey,
                Expiration = TimeSpan.FromMinutes(+1440),
                Audience = configuration.GetValue<string>("JWTSettings:Audience"),
                Issuer = configuration.GetValue<string>("JWTSettings:Issuer")
            };

            app.UseMiddleware<TokenProviderMiddleware>(tokenProviderOptionsOpt);

            return app;
        }

        public static IServiceCollection JWTAddAuthentication(this IServiceCollection services,
            IConfiguration configuration,
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            string secretKey = string.IsNullOrEmpty(configuration.GetValue<string>("JWTSettings:SecurityKey"))
                ? JwtSettings.DefaultSecretKey : configuration.GetValue<string>("JWTSettings:SecurityKey");

            services.AddAuthentication(defaultScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetValue<string>("JWTSettings:Issuer"),
                    ValidAudience = configuration.GetValue<string>("JWTSettings:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };
            });

            return services;
        }

        public static IServiceCollection JWTAddAuthentication(this IServiceCollection services,
            TokenValidationParameters tokenValidationParameters = null,
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            var tokenValidationParametersOpt = tokenValidationParameters ?? defaultOptions;

            services.AddAuthentication(defaultScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParametersOpt;
            });

            return services;
        }

        public static IServiceCollection JWTAddAuthentication(this IServiceCollection services,
            Action<JwtBearerOptions> jwtBearerOptions = null,
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            if (jwtBearerOptions == null)
            {
                jwtBearerOptions = (options) =>
                {
                    options.TokenValidationParameters = defaultOptions;
                };
            }

            services.AddAuthentication(defaultScheme).AddJwtBearer(jwtBearerOptions);

            return services;
        }

        public static IServiceCollection JWTAddAuthentication(this IServiceCollection services,
            Action<JwtBearerOptions> jwtBearerOptions = null,
            Action<AuthenticationOptions> authenticationOptions = null)
        {
            if (jwtBearerOptions == null)
            {
                jwtBearerOptions = (options) =>
                {
                    options.TokenValidationParameters = defaultOptions;
                };
            }

            if (authenticationOptions == null)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(jwtBearerOptions);
            }
            else
            {
                services.AddAuthentication(authenticationOptions).AddJwtBearer(jwtBearerOptions);
            }

            return services;
        }

        public static IServiceCollection JWTAddAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = defaultOptions;
            });

            return services;
        }

        public static IServiceCollection AddAccountManager<TAccountManager>(this IServiceCollection services)
            where TAccountManager : class, IAccountManager
        {
            services.AddScoped<IAccountManager, TAccountManager>();

            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }
    }
}
