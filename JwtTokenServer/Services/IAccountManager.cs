namespace JwtTokenServer.Services
{
    using JwtTokenServer.Models;
    using System.Threading.Tasks;

    public interface IAccountManager
    {
        Task<AccountResult> VerifyAccountAsync(string username,
            string password,
            TokenRequest tokenRequest);
    }
}
