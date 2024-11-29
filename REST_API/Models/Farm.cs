using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("farm")]
    public class Farm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("FarmManager")]
        public int FarmManagerId { get; set; }

        public string IpAddress { get; set; }

        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string Description {get; set;}

        // Navigation properties
        public FarmManager FarmManager { get; set; }

        public ICollection<FarmUnit> FarmUnits { get; set; }
        public ICollection<Facility> Facilities { get; set; }
        public ICollection<FarmCrop> FarmCrops { get; set; }
    }
}
