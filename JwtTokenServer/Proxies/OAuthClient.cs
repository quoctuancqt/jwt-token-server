namespace JwtTokenServer.Proxies
{
    using JwtTokenServer.Models;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
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
                return new JwtResult(true, JsonSerializer.Deserialize<object>(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                return new JwtResult(false, JsonSerializer.Deserialize<object>(await response.Content.ReadAsStringAsync()));
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
                return new JwtResult(true, JsonSerializer.Deserialize<object>(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                return new JwtResult(false, JsonSerializer.Deserialize<object>(await response.Content.ReadAsStringAsync()));
            }
        }
    }
}
