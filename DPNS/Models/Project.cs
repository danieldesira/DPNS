using System.ComponentModel.DataAnnotations;

namespace DPNS.Models
{
    public class Project
    {
        [Required]
        public string ProjectName { get; set; }
    }
}
