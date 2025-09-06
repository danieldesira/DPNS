using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class Notification
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
