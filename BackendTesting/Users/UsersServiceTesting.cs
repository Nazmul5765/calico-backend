using lofi_backend.Models;
using lofi_backend.Repository;
using lofi_backend.Service;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;

namespace Testing.Users
{
    internal class UsersServiceTesting
    {
        private Mock<IUserRepository> _mockRepo;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockRepo.Object);
        }

        [Test]
        public void GetUserById_ReturnsUser()
        {
            // Arrange
            var user = new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            _mockRepo.Setup(repo => repo.FetchUserById("1")).Returns(user);
            // Act
            var result = _userService.GetUserById("1");
            // Assert
            result.ShouldBe(user);
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedUser()
        {
            // Arrange
            var user = new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);

            _mockRepo.Setup(repo => repo.InsertUser(user)).Returns(user);
            var result = await _userService.CreateUser(new UserWithPassword(user, ""));

            result.ShouldBe(user);
        }

        [Test]
        public void EditUser_ReturnsUpdatedUser()
        {
            // Arrange
            var updatedUser = new UserData(id: "1", username: "Updated User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            _mockRepo.Setup(repo => repo.UpdateUser(updatedUser)).Returns(updatedUser);

            var result = _userService.EditUser(updatedUser);

            Assert.That(result, Is.EqualTo(updatedUser));
        }

        [Test]
        public void DeleteUser_CallsRepositoryDelete()
        {
            // Arrange
            var userId = "1";
            _mockRepo.Setup(repo => repo.DeleteUser(userId));
            // Act
            _userService.RemoveUser(userId);
            // Assert
            _mockRepo.Verify(repo => repo.DeleteUser(userId), Times.Once);
        }
    }
}
