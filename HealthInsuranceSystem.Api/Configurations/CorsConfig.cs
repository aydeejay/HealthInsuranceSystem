namespace HealthInsuranceSystem.Api.Configurations
{
    public static class CorsConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
        }

        public static void Configure(IApplicationBuilder app, IConfiguration config)
        {
            var origins = config["AllowedCorsDomains"].Split(",");

            app.UseCors(builder =>
            {
                builder
                    .WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    //.AllowCredentials()
                    .Build();
            });
        }
    }
}
