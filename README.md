# jwt-token-server
A JWT Authenticate library for ASP.NET Core

# How to use?
1. Create a service class to implement IAccountManager interface. This class will verify your account is valid or not. If yes, you can add more detail into Claim by using "tokenRequest.Claims.Add()" method or add more info to the result what will be returned to client side by using "tokenRequest.Responses.Add()"

  ```ruby
  
  public class AccountManager : IAccountManager
  {
      public async Task<AccountResult> VerifyAccountAsync(string username, string password, TokenRequest tokenRequest)
      {
          if (!username.Equals("admin") || !password.Equals("admin")) return new AccountResult(new { error = "Invalid user" });

          tokenRequest.Claims.Add(new CustomClaim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()));
          tokenRequest.Claims.Add(new CustomClaim(ClaimTypes.Name, "Admin"));

          tokenRequest.Responses.Add("userId", Guid.Empty.ToString());

          await Task.CompletedTask;

          return new AccountResult(tokenRequest);
      }
  }
  ```

2. Register Jwt Middleware into your project:

**For Asp.Net 2.2 using version 1.1.1**
  ```ruby
  
  public void ConfigureServices(IServiceCollection services)
  {
    services.JWTAddAuthentication(Configuration);
    
    services.AddAccountManager<AccountManager>();

    services.AddHttpClient<OAuthClient>(typeof(OAuthClient).Name, client => client.BaseAddress = new Uri("http://localhost:5000"));

    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
  }
  
  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
  {
    app.JWTBearerToken(Configuration);

    app.UseAuthentication();
    
    app.UseMvc();
  }
  
  ```
  **For Asp.Net 3.1 and upper**
  ```ruby
  
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddJWTBearerToken(Configuration);
    
    services.AddAccountManager<AccountManager>();

    services.AddHttpClient<OAuthClient>(typeof(OAuthClient).Name, client => client.BaseAddress = new Uri("http://localhost:5000"));

    services.AddCors();

    services.AddControllers().AddNewtonsoftJson();
  }
  
  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
  {
      if (env.EnvironmentName.Equals("Development"))
      {
          app.UseDeveloperExceptionPage();
      }

      app.UseJWTBearerToken(Configuration);

      app.UseRouting();

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseCors();

      app.UseEndpoints(endpoints => endpoints.MapControllers());
  }
  
  ```
  
  3. Create a controller and use OAuthClient to make a request to token server to get the access token:
  
  ```ruby
  
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

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
    {
        var response = await _oAuthClient.EnsureApiTokenAsync(dto.Username, dto.Password);

        if (response.Success) return Ok(response.Result);

        return BadRequest(response.Result);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await Task.FromResult("Hello World"));
    }
  }
  
  ```
  **After we call Login API, here is the result:**
  ``` ruby
  
  {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJBZG1pbiIsIm5iZiI6MTUzNjY0Nzk2OSwiZXhwIjoxNTM2NzM0MzY5fQ.wiRIGYxX2Kk41ix5OLSqAujEf7Stdm93kS5Ly7XXbCQ",
    "expires": 86400,
    "refreshToken": "71acb4adb95c48dba2ca7a1895957837",
    "userId": "00000000-0000-0000-0000-000000000000",
    "tokenType": "Bearer"
  }
  
  ```
