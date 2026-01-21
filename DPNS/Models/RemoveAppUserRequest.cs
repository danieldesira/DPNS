using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class RemoveAppUserRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
