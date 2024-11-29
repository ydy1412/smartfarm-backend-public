using Moq;
using Xunit;
using REST_API.Services;
using REST_API.Models;
using REST_API.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace REST_API.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly UserService _userService;
        // private readonly TokenService _tokenService;

        public UserServiceTests()
        {
            _contextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            // _userService = new UserService(_contextMock.Object,_tokenService);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldCreateUser_WhenUserDoesNotExist()
        {
            // Arrange: 유저가 존재하지 않는 경우
            var userName = "John Doe";
            var userAddress = "123 Street";
            var userPhone = "1234567890";
            var userPassword = "password123";
            var depositAmount = 100M;

            _contextMock.Setup(db => db.Users.AnyAsync(u => u.Name == userName, CancellationToken.None))
                .ReturnsAsync(false); // 유저가 존재하지 않음

            // Act: RegisterUser 호출
            var result = await _userService.RegisterUserAsync(userName, userAddress, userPhone, userPassword, depositAmount);

            // Assert: 결과가 null이 아니고, 올바른 유저가 생성되었는지 확인
            Assert.NotNull(result);
            Assert.Equal(userName, result.Name);
            Assert.Equal(userAddress, result.Address);
            Assert.Equal(userPhone, result.PhoneNumber);
            Assert.Equal(depositAmount, result.DepositAmount);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowArgumentException_WhenUserAlreadyExists()
        {
            // Arrange: 이미 유저가 존재하는 경우
            String userName = "John Doe";

            _contextMock.Setup(db => db.Users.AnyAsync(u => u.Name == userName, CancellationToken.None))
                .ReturnsAsync(true); // 유저가 이미 존재함

            // Act & Assert: 이미 존재하는 유저 등록 시 예외가 발생하는지 확인
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.RegisterUserAsync(userName, "123 Street", "1234567890", "password123", 100M)
            );
            Assert.Equal("A user with this name already exists.", exception.Message);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange: 특정 ID로 유저를 조회
            var userId = 1;
            var user = new User { Id = userId, Name = "Jane Doe" };

            _contextMock.Setup(db => db.Users.FindAsync(userId))
                .ReturnsAsync(user); // 해당 ID로 유저가 존재함

            // Act: GetUserById 호출
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert: 조회된 유저가 올바른지 확인
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("Jane Doe", result.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange: 특정 ID로 유저가 존재하지 않음
            var userId = 1;

            _contextMock.Setup(db => db.Users.FindAsync(userId))
                .ReturnsAsync((User)null); // 해당 ID로 유저가 존재하지 않음

            // Act: GetUserById 호출
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert: 결과가 null인지 확인
            Assert.Null(result);
        }

        
    }
}
