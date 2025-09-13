using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class App
    {
        [Required]
        public string AppName { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
