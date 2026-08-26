using lofi_backend.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Supabase.Functions.Interfaces;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Supabase.Postgrest.Interfaces;
using Supabase.Realtime;
using Supabase.Realtime.Interfaces;
using Supabase.Storage;
using Supabase.Storage.Interfaces;

namespace Testing.AuthControllerTesting
{
    public class ControllerTesting
    {
        private Mock<IGotrueClient<User, Session>> _mockAuthClient;
        private AuthController _authController;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockAuthClient = new Mock<IGotrueClient<User, Session>>();

            var supabaseClient = new Supabase.Client(
                _mockAuthClient.Object,
                new Mock<IRealtimeClient<RealtimeSocket, RealtimeChannel>>().Object,
                new Mock<IFunctionsClient>().Object,
                new Mock<IPostgrestClient>().Object,
                new Mock<IStorageClient<Bucket, FileObject>>().Object,
                new Supabase.SupabaseOptions());

            _httpContext = new DefaultHttpContext();
            _authController = new AuthController(supabaseClient)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }

        [Test]
        public async Task SignUp_ReturnsCreated_WhenSuccessful()
        {
            _mockAuthClient.Setup(a => a.SignUp("test@test.com", "password123", null))
                .ReturnsAsync(new Session());

            var result = await _authController.SignUp(new AuthController.LoginRequest("test@test.com", "password123"));

            result.ShouldBeOfType<CreatedResult>();
        }

        [Test]
        public async Task SignUp_ReturnsUnauthorized_WhenSupabaseReturnsNull()
        {
            _mockAuthClient.Setup(a => a.SignUp("test@test.com", "password123", null))
                .ReturnsAsync((Session)null);

            var result = await _authController.SignUp(new AuthController.LoginRequest("test@test.com", "password123"));

            result.ShouldBeOfType<UnauthorizedResult>();
        }

        [Test]
        public async Task SignUp_ReturnsUnauthorized_WhenExceptionThrown()
        {
            _mockAuthClient.Setup(a => a.SignUp("test@test.com", "password123", null))
                .ThrowsAsync(new Exception("Supabase unreachable"));

            var result = await _authController.SignUp(new AuthController.LoginRequest("test@test.com", "password123"));

            result.ShouldBeOfType<UnauthorizedResult>();
        }

        [Test]
        public async Task SignIn_ReturnsOkWithTokens_WhenSuccessful()
        {
            var session = new Session
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                User = new User { Id = "1" }
            };
            _mockAuthClient.Setup(a => a.SignInWithPassword("test@test.com", "password123"))
                .ReturnsAsync(session);

            var result = await _authController.SignIn(new AuthController.LoginRequest("test@test.com", "password123"));

            result.Result.ShouldBeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var response = okResult?.Value as AuthController.AuthResponse;
            response?.AccessToken.ShouldBe("access-token");
            response?.RefreshToken.ShouldBe("refresh-token");
        }

        [Test]
        public async Task SignIn_ReturnsNotFound_WhenSessionIsNull()
        {
            _mockAuthClient.Setup(a => a.SignInWithPassword("test@test.com", "password123"))
                .ReturnsAsync((Session)null);

            var result = await _authController.SignIn(new AuthController.LoginRequest("test@test.com", "password123"));

            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task SignIn_ReturnsNotFound_WhenTokensAreMissing()
        {
            var session = new Session { AccessToken = null, RefreshToken = null };
            _mockAuthClient.Setup(a => a.SignInWithPassword("test@test.com", "password123"))
                .ReturnsAsync(session);

            var result = await _authController.SignIn(new AuthController.LoginRequest("test@test.com", "password123"));

            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task SignOutUser_ReturnsOk()
        {
            _mockAuthClient.Setup(a => a.SignOut(Supabase.Gotrue.Constants.SignOutScope.Global))
                .Returns(Task.CompletedTask);

            var result = await _authController.SignOutUser();

            result.ShouldBeOfType<OkObjectResult>();
        }

        [Test]
        public async Task Refresh_ReturnsBadRequest_WhenNoRefreshTokenCookie()
        {
            var result = await _authController.Refresh();

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Refresh_ReturnsOkWithNewTokens_WhenSuccessful()
        {
            _httpContext.Request.Headers["Cookie"] = "refreshtoken=old-refresh-token";
            var newSession = new Session
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token"
            };
            _mockAuthClient.Setup(a => a.RefreshSession()).ReturnsAsync(newSession);

            var result = await _authController.Refresh();

            result.Result.ShouldBeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var response = okResult?.Value as AuthController.AuthResponse;
            response?.AccessToken.ShouldBe("new-access-token");
            response?.RefreshToken.ShouldBe("new-refresh-token");
        }

        [Test]
        public async Task Refresh_ReturnsBadRequest_WhenRefreshFails()
        {
            _httpContext.Request.Headers["Cookie"] = "refreshtoken=old-refresh-token";
            _mockAuthClient.Setup(a => a.RefreshSession()).ReturnsAsync((Session)null);

            var result = await _authController.Refresh();

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdatePassword_ReturnsBadRequest_WhenNewPasswordIsEmpty()
        {
            var result = await _authController.UpdatePassword(new AuthController.UpdatePasswordRequest { NewPassword = "" });

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdatePassword_ReturnsOk_WhenSuccessful()
        {
            _mockAuthClient.Setup(a => a.Update(It.IsAny<UserAttributes>())).ReturnsAsync(new User());

            var result = await _authController.UpdatePassword(new AuthController.UpdatePasswordRequest { NewPassword = "newPassword123" });

            result.ShouldBeOfType<OkObjectResult>();
        }

        [Test]
        public async Task UpdatePassword_ReturnsBadRequest_WhenUpdateReturnsNull()
        {
            _mockAuthClient.Setup(a => a.Update(It.IsAny<UserAttributes>())).ReturnsAsync((User)null);

            var result = await _authController.UpdatePassword(new AuthController.UpdatePasswordRequest { NewPassword = "newPassword123" });

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdatePassword_Returns500_WhenExceptionThrown()
        {
            _mockAuthClient.Setup(a => a.Update(It.IsAny<UserAttributes>())).ThrowsAsync(new Exception());

            var result = await _authController.UpdatePassword(new AuthController.UpdatePasswordRequest { NewPassword = "newPassword123" }) as ObjectResult;

            result?.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        [Test]
        public async Task ForgotPassword_ReturnsBadRequest_WhenEmailIsEmpty()
        {
            var result = await _authController.ForgotPassword("");

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task ForgotPassword_ReturnsOk_WhenSuccessful()
        {
            _mockAuthClient.Setup(a => a.ResetPasswordForEmail(It.IsAny<ResetPasswordForEmailOptions>()))
                .ReturnsAsync(new ResetPasswordForEmailState());

            var result = await _authController.ForgotPassword("test@test.com");

            result.ShouldBeOfType<OkResult>();
        }

        [Test]
        public async Task ForgotPassword_ReturnsBadRequest_WhenExceptionThrown()
        {
            _mockAuthClient.Setup(a => a.ResetPasswordForEmail(It.IsAny<ResetPasswordForEmailOptions>()))
                .ThrowsAsync(new Exception());

            var result = await _authController.ForgotPassword("test@test.com");

            result.ShouldBeOfType<BadRequestObjectResult>();
        }
    }
}
