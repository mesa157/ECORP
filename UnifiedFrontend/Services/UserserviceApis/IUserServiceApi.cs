using UnifiedFrontend.Models.UserModel;

namespace UnifiedFrontend.Services.UserserviceApis
{
    public interface IUserServiceApi
    {
        Task<RegisterResponseDto> RegisterUserAsync(object user); // Update to return RegisterResponseDto
        Task<string> LoginUserAsync(object credentials);
        Task<UserProfileDto> GetUserProfileAsync(Guid userId, string token);
        Task<bool> UpdateUserProfileAsync(Guid userId, UserProfileDto profile, string token);
        Task<bool> DeleteUserAccountAsync(Guid userId, string token);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, string token);
    }
}
