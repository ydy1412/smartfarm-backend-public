using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("farm_sale_offer")]
    public class FarmSaleOffer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("FarmUnit")]
        public int FarmUnitId { get; set; }
        public FarmUnit? FarmUnit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SuggestedPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public String TransactionStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties

         public ICollection<FarmSaleOrder>? FarmSaleOrders { get; set; }
    }
}
