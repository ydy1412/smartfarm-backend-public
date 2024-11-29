using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("farm_unit")]
    public class FarmUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Farm")]
        public int FarmId { get; set; }

        [ForeignKey("MetaData")]
        public int FarmUnitTypeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FarmUnitPrice { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; }
        public Farm Farm { get; set; }

        public MetaData FarmUnitType { get; set; }
        
        public ICollection<FarmSaleOffer> FarmSalesOffers { get; set; }
    }
}
