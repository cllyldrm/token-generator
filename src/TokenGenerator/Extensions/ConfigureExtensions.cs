using Microsoft.AspNetCore.Builder;

namespace TokenGenerator.Extensions
{
    public static class ConfigureExtensions
    {
        public static void UseTokenGenerator(this IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}