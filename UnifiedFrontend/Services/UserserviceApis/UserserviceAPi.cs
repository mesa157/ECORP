using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using UnifiedFrontend.Models.UserModel;

namespace UnifiedFrontend.Services.UserserviceApis
{
    public class UserServiceApi : IUserServiceApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BackendServiceOptions _options;

        public UserServiceApi(IHttpClientFactory httpClientFactory, BackendServiceOptions options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        // Register User
        public async Task<RegisterResponseDto> RegisterUserAsync(object user)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_options.UserServiceUrl}/api/users/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RegisterResponseDto>(responseContent);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (replace with your logger)
                Console.WriteLine($"Error in RegisterUserAsync: {ex.Message}");
            }

            return null;
        }

        // Login User
        public async Task<string> LoginUserAsync(object credentials)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_options.UserServiceUrl}/api/users/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(responseContent);
                    // Store the UserId for later use
                    _options.UserId = loginResponse.UserId;
                    Console.WriteLine($"User logged in with ID: {loginResponse.UserId}");
                    return loginResponse.Token;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in LoginUserAsync: {ex.Message}");
            }

            return null;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId, string token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Use the stored UserId
                var response = await client.GetAsync($"{_options.UserServiceUrl}/api/users/{_options.UserId}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserProfileDto>(responseContent);
                }
                else
                {
                    // Log non-success status code
                    Console.WriteLine($"Failed to fetch user profile. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserProfileAsync for UserId {userId}: {ex.Message}");
            }

            return null;
        }


        // Update User Profile
        public async Task<bool> UpdateUserProfileAsync(Guid userId, UserProfileDto profile, string token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{_options.UserServiceUrl}/api/users/{userId}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in UpdateUserProfileAsync: {ex.Message}");
            }

            return false;
        }

        // Delete User Account
        public async Task<bool> DeleteUserAccountAsync(Guid userId, string token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.DeleteAsync($"{_options.UserServiceUrl}/api/users/{userId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DeleteUserAccountAsync: {ex.Message}");
            }

            return false;
        }

        // Change Password
        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, string token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_options.UserServiceUrl}/api/users/{userId}/change-password", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in ChangePasswordAsync: {ex.Message}");
            }

            return false;
        }
    }

    // DTO for Registration Response
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
    }

    // DTO for Login Response
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }

    // DTO for Change Password
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
