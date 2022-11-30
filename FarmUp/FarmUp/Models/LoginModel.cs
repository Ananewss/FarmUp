using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FruitProject.Models
{
    public class LoginModel
    {
        
        public string? usr_line_id { get; set; }

        [Required]
        [Display(Name = "ชื่อ")]
        public string usr_firstname { get; set; }

        [Required]
        [Display(Name = "นามสกุล")]
        public string usr_lastname { get; set; }

        [Required]
        [Display(Name = "เบอร์โทร")]
        public string usr_phone { get; set; }

        [Required]
        [Display(Name = "ขนาดพื้นที่")]
        public string slr_farm_size { get; set; }

        [Required]
        [Display(Name = "ที่อยู่ของสวน")]
        public string slr_farm_location { get; set; }

        [Required]
        [Display(Name = "ชื่อสวน")]
        public string slr_farm_name { get; set; }

        public string? usr_latlong { get; set; }

        public string? slr_province { get; set; }

        public string? slr_district { get; set; }

        public string? slr_country { get; set; }

        public string? slr_district_th { get; set; }

        public string? slr_province_th { get; set; }

        public string? slr_country_th { get; set; }





    }
}
