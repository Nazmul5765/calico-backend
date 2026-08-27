using lofi_backend.Controllers;
using lofi_backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace Testing.UsersControllerTesting
{
    public class ControllerTesting
    {
        private Mock<IUserService> _mockService;
        private UsersController _userController;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IUserService>();
            _userController = new UsersController(_mockService.Object);
        }

        private void SetLoggedInUser(string userId)
        {
            var claims = new List<Claim> { new Claim("sub", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Test]
        public void GetUserAsync_ReturnsUser()
        {
            var expectedUser = new UserData(
                id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(expectedUser);

            var result = _userController.GetUserAsync() as ObjectResult;

            result?.StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        [Test]
        public void GetUserAsync_ReturnsNotFound()
        {
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Throws(new Exception());

            var result = _userController.GetUserAsync() as NotFoundResult;

            result?.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedUser()
        {
            var expectedUser = new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            var userToCreate =new UserWithPassword(expectedUser, "");
            SetLoggedInUser("1");
            _mockService.Setup(service => service.CreateUser(userToCreate)).ReturnsAsync(expectedUser);

            var result = await _userController.CreateUserAsync(userToCreate) as ObjectResult;

            result?.StatusCode.ShouldBe(StatusCodes.Status200OK);
            result?.Value.ShouldBe(expectedUser);
        }

        [Test]
        public async Task CreateUser_UserExists()
        {
            var userToCreate =new UserWithPassword(new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0), "");
            SetLoggedInUser("1");
            _mockService.Setup(service => service.CreateUser(userToCreate)).ThrowsAsync(new Exception());

            var result = await _userController.CreateUserAsync(userToCreate) as ObjectResult;

            result?.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            result?.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void EditUser_ReturnsUpdatedUser()
        {
            var updatedUser = new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(updatedUser);
            _mockService.Setup(service => service.EditUser(updatedUser)).Returns(updatedUser);

            var result = _userController.EditUser(updatedUser);

            result.ShouldBeOfType<OkObjectResult>();
        }

        [Test]
        public void EditUser_UserDoesNotExist()
        {
            var updatedUser = new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(updatedUser);
            _mockService.Setup(service => service.EditUser(updatedUser)).Throws(new Exception());

            var result = _userController.EditUser(updatedUser);
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void EditUser_ForbidsEditingAnotherUser()
        {
            var updatedUser = new UserData(id: "2", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            SetLoggedInUser("1");

            var result = _userController.EditUser(updatedUser);

            result.ShouldBeOfType<ForbidResult>();
        }

        [Test]
        public void GetAllUsers_ReturnsUsers_WhenAdmin()
        {
            var admin = new UserData(id: "1", username: "Admin", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0) { IsAdmin = true };
            var allUsers = new List<UserData> { admin };
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(admin);
            _mockService.Setup(service => service.GetAllUsers()).Returns(allUsers);

            var result = _userController.GetAllUsers() as ObjectResult;

            result?.StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        [Test]
        public void GetAllUsers_ForbidsNonAdmin()
        {
            var nonAdmin = new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(nonAdmin);

            var result = _userController.GetAllUsers();

            result.ShouldBeOfType<ForbidResult>();
        }

        [Test]
        public void DeleteUser_UserExists()
        {
            var admin = new UserData(id: "1", username: "Admin", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0) { IsAdmin = true };
            var deletedUser = new UserData(id: "2", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(admin);
            _mockService.Setup(service => service.RemoveUser("2")).Returns(deletedUser);

            var result = _userController.RemoveUser("2");

            result.ShouldBeOfType<NoContentResult>();
        }

        [Test]
        public void DeleteUser_UserDoesNotExist()
        {
            var admin = new UserData(id: "1", username: "Admin", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0) { IsAdmin = true };
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(admin);
            _mockService.Setup(service => service.RemoveUser("2"));

            var result = _userController.RemoveUser("2") as ObjectResult;
            result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }

        [Test]
        public void DeleteUser_ForbidsNonAdmin()
        {
            var nonAdmin = new UserData(id: "1", username: "Test User", firstName: "John", lastName: "Music", email: "email@email.com", dateOfBirth: DateTime.Now, gender: 0);
            SetLoggedInUser("1");
            _mockService.Setup(service => service.GetUserById("1")).Returns(nonAdmin);

            var result = _userController.RemoveUser("2");

            result.ShouldBeOfType<ForbidResult>();
        }
    }
}
