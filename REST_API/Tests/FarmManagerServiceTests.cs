// FarmManagerServiceTests.cs
using Moq;
using Xunit;
using REST_API.Services;
using REST_API.Models;
using REST_API.DTOs;
using REST_API.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace REST_API.Tests
{
    public class FarmManagerServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly FarmManagerService _farmManagerService;

        public FarmManagerServiceTests()
        {
            _contextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _farmManagerService = new FarmManagerService(_contextMock.Object);
        }

        [Fact]
        public async Task CreateFarmManagerAsync_ReturnsFarmManager_WhenUserExistsAndNotFarmManager()
        {
            // Arrange: 유저가 존재하고, 아직 농장 매니저가 아님
            var userId = 1;
            var user = new User { Id = userId, Name = "John Doe", IsFarmManager = false };

            _contextMock.Setup(db => db.Users.FindAsync(userId)).ReturnsAsync(user);

            // Act: FarmManager 생성 요청
            var result = await _farmManagerService.CreateFarmManagerAsync(userId);

            // Assert: 결과가 FarmManager인지 확인
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.True(user.IsFarmManager); // 유저가 이제 FarmManager로 업데이트 되었는지 확인
        }

        [Fact]
        public async Task CreateFarmManagerAsync_ThrowsException_WhenUserDoesNotExist()
        {
            // Arrange: 유저가 존재하지 않음
            var userId = 1;

            _contextMock.Setup(db => db.Users.FindAsync(userId)).ReturnsAsync((User)null); // 유저가 null

            // Act & Assert: 예외가 발생하는지 확인
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _farmManagerService.CreateFarmManagerAsync(1));
            Assert.Equal("User not found.", exception.Message);
        }

        [Fact]
        public async Task CreateFarmManagerAsync_ThrowsException_WhenUserIsAlreadyFarmManager()
        {
            // Arrange: 유저가 이미 농장 매니저임
            var userId = 1;
            var user = new User { Id = userId, Name = "Jane Doe", IsFarmManager = true };

            _contextMock.Setup(db => db.Users.FindAsync(userId)).ReturnsAsync(user);

            // Act & Assert: 예외가 발생하는지 확인
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _farmManagerService.CreateFarmManagerAsync(1));
            Assert.Equal("User is already a farm manager.", exception.Message);
        }
    }
}
