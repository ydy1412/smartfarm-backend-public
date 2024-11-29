using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REST_API.Models
{
    [Table("metadata-hierarchy")]
    public class MetaDataHierarchy
    {
        [Key]
        public int Id { get; set; } // Unique ID for this relationship (optional, if you need an ID for this table)

        [ForeignKey("ParentMetaData")]
        public int ParentId { get; set; } // 상위 메타 데이터 ID

        [ForeignKey("ChildMetaData")]
        public int ChildId { get; set; } // 하위 메타 데이터 ID

        // Navigation properties for the relationships
        public MetaData ParentMetaData { get; set; } // 상위 메타 데이터
        public MetaData ChildMetaData { get; set; } // 하위 메타 데이터
    }
}
