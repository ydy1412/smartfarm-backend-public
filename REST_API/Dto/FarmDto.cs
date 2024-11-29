using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs
{
    public class FarmCreateDto
    {
        public string name { get; set; }
        public string ipAddress { get; set; }
        public string address { get; set; }
        public int farmTypeId { get; set; }
        public string description { get; set; }
    }

}
