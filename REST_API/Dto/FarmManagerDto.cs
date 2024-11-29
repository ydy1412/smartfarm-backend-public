using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs
{
    public class FarmManagerUpdateDto
    {
        public string Name { get; set; }
        public int FarmManagerId { get; set; }
        public string IpAddress { get; set; }
        public string Address { get; set; }
        public int FarmTypeId { get; set; }
        public string Description { get; set; }
    }

}