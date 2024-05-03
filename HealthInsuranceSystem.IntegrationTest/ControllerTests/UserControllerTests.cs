using System.Net;

namespace HealthInsuranceSystem.IntegrationTest.ControllerTests
{
    public class UserControllerTests
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;

        public UserControllerTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        public async void Get_Always_ReturnsAllUsers()
        {
            var response = await _client.GetAsync("api/getAllUser");
            Assert.Equals(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}