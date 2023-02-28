using Dapper.Contrib.Extensions;

namespace FarmUp.Dtos.Product
{
    [Table("ma_user")]
    public class MaUserDBModel
    {
        [ExplicitKey]
        public int usr_id { get; set; }
        public string usr_firstname { get; set; }
        public string usr_lastname { get; set; }
        public string usr_fullname { get; set; }
        public string usr_phone { get; set; }
        public string usr_password { get; set; }
        public string usr_salt { get; set; }
        public string usr_line_id { get; set; }
        public string usr_latlong { get; set; }
        public string usr_is_active { get; set; } 
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
    }

    [Table("vw_user")]
    public class vwUserDBModel
    {
        [ExplicitKey]
        public int usr_id { get; set; }
        public string usr_firstname { get; set; }
        public string usr_lastname { get; set; }
        public string usr_fullname { get; set; }
        public string usr_phone { get; set; }
        public string usr_password { get; set; }
        public string usr_salt { get; set; }
        public string usr_line_id { get; set; }
        public string usr_latlong { get; set; }
        public string usr_is_active { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
        public string usertype { get; set; }
        public string name { get; set; }
        public string location { get; set; }
    }

    [Table("ma_buyer")]
    public class MaBuyerDBModel
    {
        [ExplicitKey]
        public int buy_id { get; set; }
        public int buy_usr_id { get; set; }
        public string buy_name { get; set; }
        public decimal? buy_size { get; set; }
        public string buy_location { get; set; }
        public string buy_district { get; set; }
        public string buy_province { get; set; }
        public string buy_country { get; set; }
        public string buy_district_th { get; set; }
        public string buy_province_th { get; set; }
        public string buy_country_th { get; set; }
        public string buy_GAP_id { get; set; }
        public string buy_GAP_file { get; set; }
        public DateTime? buy_dt_weather_noti { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
        
    }


}
