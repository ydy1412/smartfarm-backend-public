using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs
{
    public class FarmSaleOfferUpdateDto
    {   
        public decimal suggestedPrice { get; set; }
        public string transactionStatus { get; set; }
    }

}