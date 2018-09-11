namespace JwtTokenServer.Proxies
{
    using JwtTokenServer.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class OAuthClient
    {
        private readonly HttpClient _httpClient;

        public OAuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JwtResult> EnsureApiTokenAsync(string username, string password, string tokenServerPath = "/token")
        {
            HttpResponseMessage response = await _httpClient.PostAsync(tokenServerPath, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", username },
                    { "password", password }
                }));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new JwtResult(true, JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                return new JwtResult(false, JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
            }
        }

        public async Task<JwtResult> RefreshTokenAsync(string refreshTokenId, string tokenServerPath = "/token")
        {
            HttpResponseMessage response = await _httpClient.PostAsync(tokenServerPath, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshTokenId }
                }));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new JwtResult(true, JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                return new JwtResult(false, JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
            }
        }
    }
}
