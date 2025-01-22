using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using UnifiedFrontend.Models.UserModel;
using UnifiedFrontend.Services.UserserviceApis;

namespace UnifiedFrontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserServiceApi _userServiceApi;

        public AccountController(IUserServiceApi userServiceApi)
        {
            _userServiceApi = userServiceApi;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            var user = new { Name = name, Email = email, Password = password };
            var response = await _userServiceApi.RegisterUserAsync(user);

            if (response != null && response.UserId != Guid.Empty)
            {
                TempData["SuccessMessage"] = "Registration successful. Please login.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Registration failed. Please try again.";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var credentials = new { Email = email, Password = password };
            var token = await _userServiceApi.LoginUserAsync(credentials);

            if (!string.IsNullOrEmpty(token))
            {
                SaveToken(token);
                return RedirectToAction("Profile");
            }

            ViewBag.Error = "Invalid credentials. Please try again.";
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var userId = GetUserId();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login");
            }

            var profile = await _userServiceApi.GetUserProfileAsync(userId, GetToken());

            if (profile != null)
            {
                return View(profile);
            }

            ViewBag.Error = "Failed to fetch profile data.";
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileDto profile)
        {
            var userId = GetUserId();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login");
            }

            var success = await _userServiceApi.UpdateUserProfileAsync(userId, profile, GetToken());

            if (success)
            {
                return RedirectToAction("Profile");
            }

            ViewBag.Error = "Failed to update profile. Please try again.";
            return View("Profile", profile);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetUserId();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login");
            }

            var success = await _userServiceApi.DeleteUserAccountAsync(userId, GetToken());

            if (success)
            {
                ClearToken();
                return RedirectToAction("Register");
            }

            ViewBag.Error = "Failed to delete account. Please try again.";
            return View("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = GetUserId();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login");
            }

            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            var success = await _userServiceApi.ChangePasswordAsync(userId, changePasswordDto, GetToken());

            if (success)
            {
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("Profile");
            }

            ViewBag.Error = "Failed to change password. Please try again.";
            return View("Profile");
        }

        public IActionResult Logout()
        {
            ClearToken();
            return RedirectToAction("Login");
        }

        private void SaveToken(string token)
        {
            HttpContext.Session.SetString("AuthToken", token);
        }

        private string GetToken()
        {
            return HttpContext.Session.GetString("AuthToken");
        }

        private void ClearToken()
        {
            HttpContext.Session.Remove("AuthToken");
        }

        private Guid GetUserId()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return Guid.Empty;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return Guid.Empty;
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }
    }
}
