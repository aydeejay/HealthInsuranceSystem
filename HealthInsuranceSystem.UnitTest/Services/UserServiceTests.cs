using AutoMapper;

using HealthInsuranceSystem.Core.Data.PageQuery;
using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.Domain.Authorization;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;
using HealthInsuranceSystem.Core.Security;
using HealthInsuranceSystem.Core.Services;

using Moq;

using System.Linq.Expressions;

namespace HealthInsuranceSystem.Core.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private IMapper _mapper;
        private Mock<IPasswordService> _passwordServiceMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, GetUserDto>();
            }).CreateMapper();
            _passwordServiceMock = new Mock<IPasswordService>();
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _userService = new UserService(_mockUnitOfWork.Object, _mapper, _passwordServiceMock.Object, _configurationProviderMock.Object);
        }

        [Test]
        public async Task GetAllUser_ReturnsPagedResult()
        {
            // Arrange
            var query = new PaginatedQuery();
            var user = UserTestData.GetUser();
            _mockUnitOfWork.Setup(u => u.UserRepository.GetQueryable(null)).Returns(new[] { user }.AsQueryable());

            // Act
            var result = await _userService.GetAllUser(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.IsSuccessful);
            Assert.IsNotNull(result.Value.Data);
            Assert.That(result.Value.Data.PageNumber, Is.EqualTo(1));
            Assert.That(result.Value.Data.PageSize, Is.EqualTo(10));
            Assert.That(result.Value.Data.PageCount, Is.EqualTo(1));
            Assert.That(result.Value.Data.Items.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetUserByPolicyNumber_ReturnsUserDto()
        {
            // Arrange
            var policyNumber = "12345678";
            var user = UserTestData.GetUser();
            _mockUnitOfWork.Setup(u => u.UserRepository.GetFirstOrDefault(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByPolicyNumber(policyNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.IsSuccessful);
            Assert.IsNotNull(result.Value.Data);
            Assert.That(result.Value.Data.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(result.Value.Data.LastName, Is.EqualTo(user.LastName));
            Assert.That(result.Value.Data.NationalID, Is.EqualTo(user.NationalID));
            Assert.That(result.Value.Data.UserPolicyNumber, Is.EqualTo(user.UserPolicyNumber));
        }

        [Test]
        public async Task CreateUser_ReturnsSuccessfulResponse()
        {
            // Arrange
            var request = new AddUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                NationalID = "123456789",
                RoleType = RoleType.PolicyHolder,
                Password = "password123"
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetFirstOrDefault(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync((User)null);

            _mockUnitOfWork.Setup(u => u.RoleRepository.GetFirstOrDefault(It.IsAny<Expression<Func<Role, bool>>>(), null)).ReturnsAsync(new Role { RoleId = 1, Name = RoleType.PolicyHolder.ToString() });

            // Act
            var result = await _userService.CreateUser(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.IsSuccessful);
            Assert.That(result.Value.Message, Is.EqualTo(Extensions.Constants.UserConstants.Messages.UserSavedSuccessful));
        }

        [Test]
        public void GeneratePolicyHolderId_ReturnsBase64String()
        {
            // Act
            var result = UserService.GeneratePolicyHolderIdTest();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 12);
        }
    }

    public static class UserTestData
    {
        public static User GetUser()
        {
            return new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                NationalID = "123456789",
                UserPolicyNumber = "ABC123",
                IsActive = true,
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Salt = "salt123",
                HashPassword = "hash123"
            };
        }
    }

}
