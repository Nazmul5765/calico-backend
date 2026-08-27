using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;

namespace lofi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Supabase.Client _supabaseClient;

        public AuthController(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(LoginRequest user)
        {
            try
            {
                Console.WriteLine("Getting user from supabase");
                Console.WriteLine($"User Email: {user.Email}");
                Console.WriteLine($"User Password: {user.Password}");
                var result = await _supabaseClient.Auth.SignUp(user.Email, user.Password);
                    
                return result is null ? 
                    throw new UnauthorizedAccessException("Invalid credentials") : 
                    Created("User has been created successfully", user.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Unauthorized();
            }
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<AuthResponse>> SignIn(LoginRequest user)
        {
            try
            {
                Console.WriteLine("Getting user from supabase");
                var session = await _supabaseClient.Auth.SignInWithPassword(user.Email, user.Password)
                    ?? throw new UnauthorizedAccessException("No User Found");

                if (session.RefreshToken == null || session.AccessToken == null) 
                    throw new UnauthorizedAccessException("Invalid Token");

                Console.WriteLine(session.User);
                Console.WriteLine(session.User?.Id);

                Console.WriteLine(session.AccessToken);
                Console.WriteLine(session.RefreshToken);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                };
                Response.Cookies.Append("jwt", session.AccessToken, cookieOptions);
                Response.Cookies.Append("refreshtoken", session.RefreshToken, cookieOptions);
                return Ok(new AuthResponse(session.AccessToken, session.RefreshToken));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpPost("sign-out")]
        public async Task<ActionResult> SignOutUser()
        {
            await _supabaseClient.Auth.SignOut(); 
            return Ok(new { message = "logged out" });
         }
        
        [HttpPost("refresh")]
            public async Task<ActionResult<AuthResponse>> Refresh()
            {
                var refreshToken = Request.Cookies["refreshtoken"];
                if (string.IsNullOrEmpty(refreshToken)) 
                    return BadRequest("Refresh token is missing.");

                var newToken = await _supabaseClient.Auth.RefreshSession();
                if (newToken?.RefreshToken is null || newToken.AccessToken is null)
                    return BadRequest("Failed to refresh token.");

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                };
                Response.Cookies.Append("jwt", newToken.AccessToken, cookieOptions);
                Response.Cookies.Append("refreshtoken", newToken.RefreshToken, cookieOptions);

                return Ok(new AuthResponse(newToken.AccessToken, newToken.RefreshToken));
            }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { message = "New password cannot be empty." });
            }
            try
            {
                var attributes = new UserAttributes
                {
                    Password = request.NewPassword
                };
                var updatedUserPassword = await _supabaseClient.Auth.Update(attributes);
                if (updatedUserPassword != null)
                {
                    Console.WriteLine("Password updated successfully.");
                    return Ok(new { message = "Password updated successfully." });
                }
                return BadRequest(new { message = "Failed to update password." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password update email: {ex.Message}");
                return StatusCode(500, new { message = "Error updating password." });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            // email will be the raw JSON string, e.g. "example@gmail.com"
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest("Email is required.");
                }

                Console.WriteLine($"Received email for password reset: {email}");

                var options = new ResetPasswordForEmailOptions(email)
                {
                    RedirectTo = "https://localhost:5082/Login"
                };

                await _supabaseClient.Auth.ResetPasswordForEmail(options);

                Console.WriteLine("Password reset email sent successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                return BadRequest("Error sending password reset email.");
            }
        }

        public class UpdatePasswordRequest
        {
            public string NewPassword { get; set; } = string.Empty;
        }

        public record LoginRequest(string Email, string Password);
        
        public class AuthResponse(string token, string refresh)
        {
            public string AccessToken { get; set; } = token;

            public string RefreshToken { get; set; } = refresh;
        }
    }
}
