using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class EmailVerificationRequest
    {
        [Required]
        public string VerificationCode { get; set; }
    }
}
