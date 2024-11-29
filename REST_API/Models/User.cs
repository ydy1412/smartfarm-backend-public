using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // 유저가 팜 매니저인지 여부를 확인하는 칼럼 추가
        public bool IsFarmManager { get; set; } = false; // 기본값은 false (팜 매니저 아님)

        // Navigation properties
        public FarmManager FarmManager { get; set; }
        public ICollection<FarmUnit> FarmUnits { get; set; }
        public ICollection<FarmSaleOrder> FarmSaleOrders { get; set; }
    }
}
