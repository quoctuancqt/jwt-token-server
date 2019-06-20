namespace JwtTokenServer.Middlewares
{
    using JwtTokenServer.Enums;
    using JwtTokenServer.Models;
    using JwtTokenServer.Services;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class TokenProviderMiddleware
    {
        public readonly RequestDelegate _next;

        public readonly TokenProviderOptions _options;

        public TokenProviderMiddleware(RequestDelegate next, TokenProviderOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
            {

                await BadRequest(context, new { error = "Bad request.." });

                return;
            }

            GrantTypeEnum grantType = GrantTypeEnum.password;

            try
            {
                grantType = (GrantTypeEnum)Enum.Parse(typeof(GrantTypeEnum), context.Request.Form["grant_type"], true);
            }
            catch
            {
                await BadRequest(context, new { error = "invail grant_type" });

                return;
            }

            switch (grantType)
            {
                case GrantTypeEnum.password:
                    await GenerateTokenByUserNamePassWord(context, context.Request.Form["username"],
                        context.Request.Form["password"]);
                    break;
                case GrantTypeEnum.refresh_token:
                    await GenerateTokenByRefreshToken(context, context.Request.Form["refresh_token"]);
                    break;
            }
        }

        #region PRIVATE METHOD
        private async Task GenerateTokenByUserNamePassWord(HttpContext context,
            string username,
            string password)
        {
            var customClaims = new List<CustomClaim>();

            customClaims.Add(new CustomClaim("iss", _options.Issuer));

            var tokenRequest = new TokenRequest(_options, customClaims);

            IServiceProvider serviceProvider = context.RequestServices;

            var accountManager = (IAccountManager)serviceProvider.GetService(typeof(IAccountManager));

            var tokenService = (ITokenService)serviceProvider.GetService(typeof(ITokenService));

            var accountResult = new AccountResult();

            accountResult = await accountManager.VerifyAccountAsync(username,
                    password, tokenRequest);

            if (accountResult.Successed)
            {
                context.Response.StatusCode = 200;

                context.Response.ContentType = "application/json";

                var token = tokenService.GenerateToken(accountResult.TokenRequest);

                await context.Response.WriteAsync(ObjToJson(token));

                return;
            }
            else
            {
                context.Response.StatusCode = 400;

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(ObjToJson(accountResult.Error));

                return;
            }
        }

        private async Task GenerateTokenByRefreshToken(HttpContext context, string refreshToken)
        {
            try
            {
                IServiceProvider serviceProvider = context.RequestServices;

                var tokenService = (ITokenService)serviceProvider.GetService(typeof(ITokenService));

                context.Response.StatusCode = 200;

                context.Response.ContentType = "application/json";

                var token = tokenService.RefreshAccessToken(refreshToken);

                await context.Response.WriteAsync(ObjToJson(token));

                return;

            }
            catch (Exception ex)
            {
                await BadRequest(context, new { error = ex.Message });

                return;
            }
        }

        private async Task BadRequest(HttpContext context, object msg)
        {
            context.Response.StatusCode = 400;

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(ObjToJson(msg));

            return;
        }

        private string ObjToJson<TModel>(TModel model)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(model, jsonSettings);
        }
        #endregion
    }
}
