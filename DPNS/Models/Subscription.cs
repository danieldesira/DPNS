using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class Subscription
    {
        [Required]
        public string Endpoint { get; set; }

        [Required]
        public SubscriptionKeys Keys { get; set; }
    }

    public class SubscriptionKeys
    {
        [Required]
        public string P256dh { get; set; }

        [Required]
        public string Auth { get; set; }
    }
}
