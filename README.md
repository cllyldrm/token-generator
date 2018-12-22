# Token Generator

You can generate jwt tokens, refresh tokens and decode jwt tokens with this library on .net core projects.

## Getting Started

### Using

TokenSettings class is need for jwt token settings. You must implement in your appsettings.

````csharp
    {
      "TokenSettings": {
          "Audience": "your audience",
          "Issuer": "your issuer",
          "SigningKey": "your signing key",
          "ExpireMinute": 120
      }
  }
````

The next step is adding some extensions into startup class.

````csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        services.AddTokenGenerator(configuration);
    }
````

````csharp
    public void Configure(IApplicationBuilder app)
    {
        app.UseTokenGenerator();
        app.UseMvc();
    }
````

Configurations are done. Now, you can start using features.

````csharp
    private readonly ITokenManager _tokenManager;

    public Test(ITokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    public void Usage()
    {
        var token = _tokenManager.GenerateToken(new Dictionary<string, object>
        {
            {
                "userId", Guid.NewGuid()
            },
            {
                "userEmail", "test@gmail.com"
            },
            {
                "nameAndSurname", "Test Test"
            }
        });

        var refreshToken = _tokenManager.GenerateRefreshToken();

        var claims = _tokenManager.GetClaims(token);

        if (claims != null)
        {
            var userId = claims["userId"];
        }
    }
````

That' s it!
