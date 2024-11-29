using BCrypt.Net;

namespace REST_API.Utilities
{
    public static class PasswordHasher
    {
        // 비밀번호 해싱
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // 비밀번호 검증
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}