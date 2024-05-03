using HealthInsuranceSystem.Core.Data.Repository.IRepository;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace HealthInsuranceSystem.IntegrationTest.ControllerTests
{
    internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private Mock<IUserRepository> _userRepositoryMock;

        public CustomWebApplicationFactory()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_userRepositoryMock.Object);
            });
        }
    }
}