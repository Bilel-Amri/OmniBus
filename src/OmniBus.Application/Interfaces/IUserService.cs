using System.Threading.Tasks;
using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<UserDto?> UpdateUserAsync(Guid id, UserProfileDto updateDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<UserDto?> LoginAsync(string email, string password);
        Task<bool> ChangePasswordAsync(Guid id, string currentPassword, string newPassword);
    }
}