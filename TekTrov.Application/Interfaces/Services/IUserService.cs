using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Auth;
using TekTrov.Application.DTOs.Users;

namespace TekTrov.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);

        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);

        Task<UserResponseDTO?> GetUserByIdAsync(int id);

        Task BlockUserAsync(int userId);
        Task UnblockUserAsync(int userId);

        Task ChangePasswordAsync(int userId, ChangePasswordDTO dto);

        Task SendPasswordOtpAsync(SendPasswordOtpDTO dto);
        Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpDTO dto);

        Task UpdateWishlistAsync(int userId, List<int> productIds);




    }
}
