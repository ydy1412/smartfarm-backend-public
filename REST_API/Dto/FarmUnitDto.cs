using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs
{
    public class FarmUnitCreateDto
{
    public string Name { get; set; }
    public decimal FarmUnitPrice { get; set; }
    public int FarmUnitTypeId {get; set;}
    public string Description { get; set; }
}

}
