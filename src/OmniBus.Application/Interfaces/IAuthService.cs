using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserResponseDto> GetCurrentUserAsync(Guid userId);
    Task<UserResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileDto request);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto request);
    Task<bool> ValidateTokenAsync(string token);
    Guid? GetUserIdFromToken(string token);
}
