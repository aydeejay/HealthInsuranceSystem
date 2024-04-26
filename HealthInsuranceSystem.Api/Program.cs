using HealthInsuranceSystem.Api.Configurations;

using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);
var service = builder.Services;
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AutoMapperConfig.Configure(service);
CorsConfig.ConfigureServices(service);
AuthorizationConfig.ConfigureServices(service, config);
AuthenticationConfig.ConfigureServices(service, config);
DatabaseConfig.ConfigureServices(service, config);
IdentityServerConfig.ConfigureServices(service, config);
FluentValidationConfig.Configure();
ServiceExtensionConfig.ConfigureServices(service);
SwaggerConfig.ConfigureServices(service, config);
IdentityModelEventSource.ShowPII = true;

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
SwaggerConfig.Configure(app, config);
CorsConfig.Configure(app, config);
AuthenticationConfig.Configure(app);
DatabaseConfig.Configure(app);
IdentityServerConfig.Configure(app);
DefaultData.Initialize(app);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
