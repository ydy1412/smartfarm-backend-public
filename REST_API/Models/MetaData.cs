using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("metadata")]
    public class MetaData
    {
        [Key]
        public int Id { get; set; } // Unique ID for MetaData

        [Required]
        public int Level { get; set; } // 계층 (1, 2, 3, 4)

        [Required]
        [MaxLength(100)]
        public string TypeCode { get; set; } // 속성 유형 (예: 아두이노, 센서, 액츄에이터, 작물)

        [Required]
        [MaxLength(200)]
        public string Value { get; set; } // 메타 데이터 값 (예: Arduino Uno, DHT22, Lettuce)

        [MaxLength(500)]
        public string Description { get; set; } ="";// 설명

        public int? ParentMetaDataId { get; set; } // 상위 메타 데이터 ID (null이면 최상위 계층)

        [ForeignKey("ParentMetaDataId")]
        public MetaData ParentMetaData { get; set; } // 상위 메타 데이터 (자신을 참조하는 계층의 부모)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 생성 시간

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // 수정 시간

        [InverseProperty("ParentMetaData")]
        public ICollection<MetaData> ChildMetaData { get; set; } = new List<MetaData>(); // 하위 메타 데이터
    }
}
