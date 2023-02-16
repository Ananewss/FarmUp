using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherRobot
{
    public class ma_location_Item
    {
        public string loc_id { get; set; }
		public string loc_SubDistrict { get; set; }
        public string loc_District { get; set; }
        public string loc_Province { get; set; }
        public string loc_Country { get; set; }
        public string loc_weather_url { get; set; }
    }
}
