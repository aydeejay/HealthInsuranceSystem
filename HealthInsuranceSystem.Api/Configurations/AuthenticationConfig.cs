using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HealthInsuranceSystem.Api.Configurations
{
    public static class AuthenticationConfig
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = config["Identity:Authority"];
                    options.Audience = IdentityServerConfig.ApiResourceName;
                    options.RequireHttpsMetadata = config.GetValue<bool>("Identity:RequireHttps");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireSignedTokens = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateTokenReplay = false
                    };
                });
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}
