using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TokenGenerator.Managers;
using TokenGenerator.Managers.Interfaces;
using TokenGenerator.Models;

namespace TokenGenerator.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static void AddTokenGenerator(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenSettings = configuration.GetSection("TokenSettings");
            services.Configure<TokenSettings>(tokenSettings);

            services.AddTransient<ITokenManager, TokenManager>();

            services.AddAuthentication(options =>
                                       {
                                           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       }
                                      )
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = true,
                            ValidAudience = tokenSettings["Audience"],
                            ValidateIssuer = true,
                            ValidIssuer = tokenSettings["Issuer"],
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings["SigningKey"])),
                            ClockSkew = TimeSpan.Zero
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnChallenge = context => throw new SecurityTokenExpiredException()
                        };
                    });
        }
    }
}