using TekTrov.Application.DTOs.Auth;
using TekTrov.Application.DTOs.Users;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Domain.Entities;
using TekTrov.Domain.Enums;


namespace TekTrov.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public UserService(
     IUserRepository userRepository,
     IJwtService jwtService,
     IEmailService emailService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _emailService = emailService;
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
                Role = Roles.User
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


        //public async Task BlockUserAsync(int userId)
        //{
        //    var user = await _userRepository.GetByIdAsync(userId)
        //        ?? throw new Exception("User not found");

        //    if (user.IsBlocked)
        //        throw new Exception("User already blocked");

        //    user.IsBlocked = true;
        //    await _userRepository.UpdateAsync(user);
        //}

        //public async Task UnblockUserAsync(int userId)
        //{
        //    var user = await _userRepository.GetByIdAsync(userId)
        //        ?? throw new Exception("User not found");

        //    if (!user.IsBlocked)
        //        throw new Exception("User already unblocked");

        //    user.IsBlocked = false;
        //    await _userRepository.UpdateAsync(user);
        //}
        public async Task<bool> ToggleBlockUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            user.IsBlocked = !user.IsBlocked;

            await _userRepository.UpdateAsync(user);

            return user.IsBlocked;
        }


        public async Task ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            if (dto.NewPassword.StartsWith(" "))
                throw new Exception("Leading spaces are not allowed");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            var isValidPassword = BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword, user.Password);

            if (!isValidPassword)
                throw new Exception("Current password is incorrect");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                dto.NewPassword,
                workFactor: 12
            );

            user.Password = hashedPassword;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userRepository.UpdateAsync(user);
        }

        public async Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpDTO dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user = await _userRepository
                .GetByEmailOtpAsync(email, dto.Otp)
                ?? throw new Exception("Invalid or expired OTP");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                dto.NewPassword, workFactor: 12);

            user.Password = hashedPassword;

            user.EmailOtp = null;
            user.EmailOtpExpiry = null;
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userRepository.UpdateAsync(user);
        }


        public async Task SendPasswordOtpAsync(SendPasswordOtpDTO dto)
        {
            var email = dto.Email.Trim().ToLower();
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return;

            var otp = new Random().Next(100000, 999999).ToString();

            user.EmailOtp = otp;
            user.EmailOtpExpiry = DateTime.UtcNow.AddMinutes(5);

            await _userRepository.UpdateAsync(user);

            await _emailService.SendAsync(
                email,
                "TekTrov Password Reset OTP",
                $"<h3>Your OTP is <b>{otp}</b></h3><p>Valid for 5 minutes.</p>"
            );
        }

        public async Task UpdateWishlistAsync(int userId, List<int> productIds)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            user.Wishlists.Clear();

            foreach (var productId in productIds)
            {
                user.Wishlists.Add(new Wishlist
                {
                    ProductId = productId,
                    UserId = userId
                });
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateProfileAsync(int userId, UpdateProfileDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            user.Name = dto.Name.Trim();
            user.Email = dto.Email.Trim().ToLower();

            await _userRepository.UpdateAsync(user);
        }

        public async Task<List<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    IsBlocked = u.IsBlocked,
                    CreatedOn = u.CreatedOn
                })
                .ToList();
        }

    }

}