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

            //services.AddEntityFrameworkNpgsql().AddDbContext<DataContext>(opt =>
            //    opt.UseNpgsql(config.GetConnectionString("Default"), sql => sql.MigrationsAssembly("HealthInsuranceSystem.Data")));
            //opt.UseNpgsql(config.GetConnectionString("DataContext"), sql => sql.UseNetTopologySuite()));

        }

        public static void Configure(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DataContext>();
                //var container = serviceScope.ServiceProvider.GetService<Container>();
                //context.Database.EnsureCreated();
                context.Database.Migrate();
            }
        }
    }
}
