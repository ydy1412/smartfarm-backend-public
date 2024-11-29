using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("facility")]
    public class Facility
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Farm")]
        public int FarmId { get; set; }

        [ForeignKey("MetaData")]
        public int FacilityTypeId { get; set; } // 시설 종류를 메타 데이터로 관리

        public string Description { get; set; } // 시설에 대한 설명

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Farm Farm { get; set; }
        public MetaData FacilityType { get; set; }
    }
}
