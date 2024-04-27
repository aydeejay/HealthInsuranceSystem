using HealthInsuranceSystem.Core.Data;

using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Api.Configurations
{
    public class DatabaseConfig
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<Core.Data.DataContext, DataContext>();

            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(config.GetConnectionString("Default"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }));
        }

        public static void Configure(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DataContext>();
                //context.Database.EnsureCreated();
                context.Database.Migrate();
            }
        }
    }
}
