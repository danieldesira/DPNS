using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class RemoveSubscriptionRequest
    {
        [Required]
        public string Endpoint { get; set; }
    }
}
