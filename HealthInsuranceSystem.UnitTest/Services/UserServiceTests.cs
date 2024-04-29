using AutoMapper;

using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Data.PageQuery;
using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;
using HealthInsuranceSystem.Core.Security;
using HealthInsuranceSystem.Core.Services;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace HealthInsuranceSystem.Core.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<DataContext> _contextMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<DataContext>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _configurationProviderMock = new Mock<IConfigurationProvider>();

            _userService = new UserService(_unitOfWorkMock.Object, _mapperMock.Object, _contextMock.Object, _passwordServiceMock.Object, _configurationProviderMock.Object);
        }

        [Test]
        public async Task GetAllUser_ReturnsPagedResult_WhenValidQuery()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe", NationalID = "123456789", UserPolicyNumber = "ABC123" },
                new User { Id = 2, FirstName = "Jane", LastName = "Smith", NationalID = "987654321", UserPolicyNumber = "DEF456" }
            };

            _contextMock.Setup(c => c.Set<User>().AsNoTracking()).Returns((Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, Models.Domain.Authorization.Role>)users.AsQueryable());

            var query = new PaginatedQuery
            {
                PageQuery = new PagedQueryRequest
                {
                    PageNumber = 1,
                    PageSize = 2
                }
            };

            // Act
            var result = await _userService.GetAllUser(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.That(result.Value.Data.TotalItemCount, Is.EqualTo(2));
            Assert.That(result.Value.Data.Items.Count, Is.EqualTo(2));
            Assert.That(result.Value.Data.Items[0].FirstName, Is.EqualTo("John"));
            Assert.That(result.Value.Data.Items[1].FirstName, Is.EqualTo("Jane"));
        }

        [Test]
        public async Task GetUserByPolicyNumber_ReturnsUserDto_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", NationalID = "123456789", UserPolicyNumber = "ABC123" };
            _contextMock.Setup(c => c.Set<User>().AsNoTracking().Include(x => x.Role)).Returns((Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, Models.Domain.Authorization.Role>)new List<User> { user }.AsQueryable());

            // Act
            var result = await _userService.GetUserByPolicyNumber("ABC123");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.IsSuccessful);
            Assert.That(result.Value.Data.FirstName, Is.EqualTo("John"));
            Assert.That(result.Value.Data.LastName, Is.EqualTo("Doe"));
            Assert.That(result.Value.Data.NationalID, Is.EqualTo("123456789"));
            Assert.That(result.Value.Data.UserPolicyNumber, Is.EqualTo("ABC123"));
        }

        [Test]
        public async Task CreateUser_ReturnsSuccessfulResponse_WhenValidUserData()
        {
            // Arrange
            var request = new AddUserDto { NationalID = "123456789", FirstName = "John", LastName = "Doe", RoleType = RoleType.PolicyHolder, Password = "password123" };
            _contextMock.Setup(c => c.Set<User>().AsNoTracking().Include(x => x.Role)).Returns((Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, Models.Domain.Authorization.Role>)new List<User>().AsQueryable());
            _passwordServiceMock.Setup(p => p.CreateHash(request.Password, It.IsAny<string>())).Returns("hashedPassword123");

            // Act
            var result = await _userService.CreateUser(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.IsSuccessful);
            Assert.That(result.Value.Message, Is.EqualTo(Extensions.Constants.UserConstants.Messages.UserSavedSuccessful));
        }

        [Test]
        public async Task CreateUser_ReturnsFailureResponse_WhenUserAlreadyExists()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", NationalID = "123456789", UserPolicyNumber = "ABC123" };
            _contextMock.Setup(c => c.Set<User>().AsNoTracking().Include(x => x.Role)).Returns((Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, Models.Domain.Authorization.Role>)new List<User> { user }.AsQueryable());

            // Act
            var result = await _userService.CreateUser(new AddUserDto { NationalID = "123456789", FirstName = "John", LastName = "Doe", RoleType = RoleType.PolicyHolder, Password = "password123" });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Value.IsSuccessful);
            Assert.That(result.Value.Message, Is.EqualTo(Extensions.Constants.UserConstants.ErrorMessages.UserExists));
        }

        // a helper to make dbset queryable
        private Mock<DataContext> GetMockDbSet<T>(IQueryable<T> entities) where T : class
        {
            var mockSet = new Mock<DataContext>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());
            return mockSet;
        }
    }
}
