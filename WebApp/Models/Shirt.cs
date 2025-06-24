using System.ComponentModel.DataAnnotations;
using WebApp.Models.Validations;

namespace WebApp.Models
{
    public class Shirt
    {
        public int shirtId { get; set; }
        [Required]
        public string? Brand { get; set; }
        [Required]
        public string? color { get; set; }
        [shirt_InsureCorrectSizing]
        public int? Size { get; set; }
        [Required]
        public string? Gender { get; set; }
        public double? Price { get; set; }
    }
}
