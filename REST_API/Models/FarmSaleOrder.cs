using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("farm_sale_order")]
    public class FarmSaleOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("FarmSalesOffer")]
        public int FarmSaleOfferId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ApprovalStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; }

        // Navigation properties
        public FarmSaleOffer FarmSalesOffer { get; set; }
    }
}
