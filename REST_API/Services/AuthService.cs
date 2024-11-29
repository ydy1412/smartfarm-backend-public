using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using REST_API.Models;

namespace REST_API.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            // new Claim(ClaimTypes.Name, user.Username),
            // new Claim(ClaimTypes.Role, user.Role) // 유저 권한 추가 (예: Admin, User)
        };

            var token = new JwtSecurityToken(
                // issuer: _config["JwtSettings:Issuer"],
                // audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(10), // 토큰 유효 시간 설정
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
