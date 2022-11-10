using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace WeatherRobot
{
    public class tr_weather_day_Item
    {
        public string Wea_Id { get; set; }
		public DateTime DT { get; set; }
		public string Temp { get; set; }
		public string Wind { get; set; }
		public string Humidity { get; set; }
        public string UV { get; set; }
        public string CloudCover { get; set; }
        public string RainAmt { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
    }
}
