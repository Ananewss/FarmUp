using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FruitProject.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
