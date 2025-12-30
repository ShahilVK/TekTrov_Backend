using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Application.DTOs.Auth;
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

        //public async Task RegisterAsync(RegisterDTO dto)
        //{
        //    var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        //    if (existingUser != null)
        //        throw new Exception("User already exists");

        //    var user = new User
        //    {
        //        Name = dto.Name,
        //        Email = dto.Email,
        //        Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        //        Role = "User"
        //    };

        //    await _userRepository.AddAsync(user);
        //}


        public async Task RegisterAsync(RegisterDTO dto)
        {
            var name = dto.Name.Trim();
            var email = dto.Email.Trim().ToLower();

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            var user = new User
            {
                Name = name,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            await _userRepository.AddAsync(user);
        }
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid email or password");

            var accessToken = _jwtService.GenerateAccessToken(
                user.Id, user.Email, user.Role);

            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

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
    }
}