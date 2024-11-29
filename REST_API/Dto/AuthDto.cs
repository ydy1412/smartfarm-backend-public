using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs
{
    public class UserLoginDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}

}
