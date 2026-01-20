using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class AddAppUserRequest
    {
        [Required]
        public Guid AppGuid { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
