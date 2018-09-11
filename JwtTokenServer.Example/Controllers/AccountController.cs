using System.Threading.Tasks;
using JwtTokenServer.Example.Dto;
using JwtTokenServer.Proxies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtTokenServer.Example.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly OAuthClient _oAuthClient;

        public AccountController(OAuthClient oAuthClient)
        {
            _oAuthClient = oAuthClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult("Hello World"));
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
        {
            var response = await _oAuthClient.EnsureApiTokenAsync(dto.Username, dto.Password);

            if (response.Success) return Ok(response.Result);

            return BadRequest(response.Result);
        }
    }
}
