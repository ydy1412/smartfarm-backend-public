using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [Range(0, 1000000)]
        public decimal DepositAmount { get; set; }
    }

    public class UserEditDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [Range(0, 1000000)]
        public decimal DepositAmount { get; set; }
    }
}
