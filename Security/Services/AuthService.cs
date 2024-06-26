﻿using Microsoft.IdentityModel.Tokens;
using Security_CSharp.Exceptions;
using Security_CSharp.Security.DTOs;
using Security_CSharp.Security.Entitites;
using Security_CSharp.Security.Interfaces;
using Security_CSharp.Security.Mappers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Security_CSharp.Security.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        // Change if necesesarry
        private readonly int EXPIRATION_HOURS = 2;
        // Change to null to not assign a default role on register
        private readonly string DEFAULT_ROLENAME = "USER";

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }

        public async Task<UserResponse> register(SignupRequest request)
        {
            var userDbEmail = await _userRepository.GetUserByEmail(request.Email);
            var userDbUsername = await _userRepository.GetUserByUsername(request.Username);

            if (userDbEmail is not null) throw new BadRequestException($"User with email {request.Email} exists already.");
            if (userDbUsername is not null) throw new BadRequestException($"User with username {request.Username} exists already.");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new User()
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await SetDefaultRole(newUser);

            var createdUser = await _userRepository.CreateUser(newUser);
            return createdUser.ToDTOUser();
        }


        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var userDb = await _userRepository.GetUserByUsername(request.Username) ?? throw new BadRequestException("Wrong username or password");
            if (!VerifyPasswordHash(request.Password, userDb.PasswordHash, userDb.PasswordSalt)) throw new BadRequestException("Wrong username or password");



            return new LoginResponse() { Username = userDb.Username, Token = CreateToken(userDb), Roles = userDb.Roles.Select(r => r.Name) };
        }

        private async Task SetDefaultRole(User user)
        {
            if (DEFAULT_ROLENAME is null) return;

            var roleToAssign = await _roleRepository.GetRoleByName(DEFAULT_ROLENAME) ?? throw new NotFoundException($"Default role not found in db. Value: {DEFAULT_ROLENAME}");

            user.Roles.Add(roleToAssign);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("iss", "almo.kea"),
                new Claim("subject", user.Username),
                new Claim("mail", user.Email),
                new Claim("roles", string.Join(", ", user.Roles.Select(r => r.Name)) ?? ""),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            // TokenSecret is stored in user secret AppSettings:TokenSecret
            var tokenSecret = _configuration.GetSection("AppSettings:TokenSecret").Value ?? throw new Exception("TokenSecret is not set.");

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenPayload = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(EXPIRATION_HOURS),
                signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenPayload);

            return jwt;
        }

        private void CreatePasswordHash(string plainPassword, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(plainPassword));
            }
        }

        private bool VerifyPasswordHash(string inputtedPassword, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputtedPassword));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
