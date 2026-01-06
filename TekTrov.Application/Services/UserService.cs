using TekTrov.Application.DTOs.Auth;
using TekTrov.Application.DTOs.Users;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;


namespace TekTrov.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UserService(
            IUserRepository userRepository,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }



        public async Task RegisterAsync(RegisterDTO dto)
        {
            if (dto.Name.StartsWith(" ") ||
                dto.Email.StartsWith(" ") ||
                dto.Password.StartsWith(" "))
                throw new Exception("Leading spaces are not allowed");

            var email = dto.Email.ToLower();

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                dto.Password,
                workFactor: 12
            );

            var user = new User
            {
                Name = dto.Name,
                Email = email,
                Password = hashedPassword,
                Role = "User"
            };

            await _userRepository.AddAsync(user);
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid email or password");

            if (user.IsBlocked)
                throw new Exception("Your account has been blocked by admin");

            var accessToken = _jwtService.GenerateAccessToken(
                user.Id, user.Email, user.Role);

            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAsync(user);

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }


        public async Task LogoutAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            user.RefreshToken = null;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
                return null;

            return new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedOn = user.CreatedOn
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
                throw new Exception("Invalid or expired refresh token");

            var newAccessToken = _jwtService.GenerateAccessToken(
                user.Id, user.Email, user.Role
            );

            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAsync(user);

            return new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }


        public async Task BlockUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            if (user.IsBlocked)
                throw new Exception("User already blocked");

            user.IsBlocked = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task UnblockUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            if (!user.IsBlocked)
                throw new Exception("User already unblocked");

            user.IsBlocked = false;
            await _userRepository.UpdateAsync(user);
        }

    }

}