using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("farm_crop")]
    public class FarmCrop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Farm")]
        public int FarmId { get; set; } // 팜에 대한 외래키

        [ForeignKey("MetaData")]
        public int CropTypeId { get; set; } // 메타데이터의 작물 유형을 참조하는 외래키

        public string Description {get;set;} ="";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Farm Farm { get; set; }
        public MetaData CropType { get; set; } // 작물 유형 메타 데이터 (예: 토마토, 상추 등)
    }
}
