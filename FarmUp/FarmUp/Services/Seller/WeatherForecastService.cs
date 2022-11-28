using FarmUp.Dtos.Seller;
using MySql.Data.MySqlClient;
using System.Text;
using MySql.Data;

namespace FarmUp.Services.Seller
{
    public class WeatherForecastService
    {
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly IConfiguration _config;
        public WeatherForecastService(ILogger<WeatherForecastService> logger,
                                             IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<WeatherForecastDtoList> GetWeatherForecastByLocation(string subdistrict,string district,string province)
        {
            WeatherForecastDtoList weatherForecastDtoList = new WeatherForecastDtoList();
            StringBuilder sbReadWeather = new StringBuilder();

            /*
                SELECT * FROM tr_weather_day
                WHERE SubDistrict IN ('บ้านนา')
                AND District IN ('แกลง')
                AND Province IN ('ระยอง')
                AND DATE(Created_at) IN ('2022-11-06')
                AND isActive = 'Y'
                ORDER BY Created_at
             */
            sbReadWeather.Append($"SELECT * FROM tr_weather_day ");
            sbReadWeather.Append($"WHERE SubDistrict IN ('Khlung') ");
            sbReadWeather.Append($"AND District IN ('Khlung') ");
            sbReadWeather.Append($"AND Province IN ('Chanthaburi') ");
            sbReadWeather.Append($"AND DATE(DT) IN (CURDATE(), CURDATE() + INTERVAL 1 DAY) ");
            sbReadWeather.Append($"ORDER BY DT");
            _logger.LogInformation($"[WeatherForecastService][GetWeatherForecastByLocation][sbReadWeather] = {sbReadWeather.ToString()}");
            string strConn = _config.GetConnectionString($"onedurian");

            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            try
            {
                
                MySqlCommand mCmd = new MySqlCommand(sbReadWeather.ToString(), mSqlConn);

                await mSqlConn.OpenAsync();
                var readData = await mCmd.ExecuteReaderAsync();
                while (await readData.ReadAsync())
                {
                    WeatherForecastDto weatherForecastDto = new WeatherForecastDto();
                    weatherForecastDto.Wea_Id = "--";
                    weatherForecastDto.DT = (DateTime) readData["DT"];
                    weatherForecastDto.Temp = readData["Temp"].ToString() ?? "";
                    weatherForecastDto.Wind = readData["Wind"].ToString() ?? "";
                    weatherForecastDto.Humidity = readData["Humidity"].ToString() ?? "";
                    weatherForecastDto.UV = readData["UV"].ToString() ?? "";
                    weatherForecastDto.CloudCover = readData["CloudCover"].ToString() ?? "";
                    weatherForecastDto.RainAmt = readData["RainAmt"].ToString() ?? "";
                    weatherForecastDto.SubDistrict = readData["SubDistrict"].ToString() ?? "";
                    weatherForecastDto.District = readData["District"].ToString() ?? "";
                    weatherForecastDto.Province = readData["Province"].ToString() ?? "";

                    weatherForecastDto.Icon = (Int32.Parse(weatherForecastDto.RainAmt.Trim('%')) >= 30) ? "rain.png" : (weatherForecastDto.DT.Hour > 18 || weatherForecastDto.DT.Hour < 6) ? "moon.png" : "sun.png";
                    weatherForecastDto.CloudyMsg = (Int32.Parse(weatherForecastDto.CloudCover) >= 60) ? "เมฆมาก" : (Int32.Parse(weatherForecastDto.CloudCover) >= 30) ? "เมฆปานกลาง" : "เมฆน้อย";

                    weatherForecastDtoList.weatherForecastDtosList.Add(weatherForecastDto);

                    weatherForecastDtoList.SubDistrict = readData["SubDistrict"].ToString() ?? "";
                    weatherForecastDtoList.District = readData["District"].ToString() ?? "";
                    weatherForecastDtoList.Province = readData["Province"].ToString() ?? "";
                }
                
            }
            catch(Exception ex)
            {

            }
            finally
            {
                await mSqlConn.CloseAsync();
            }
           
            return await Task.FromResult(weatherForecastDtoList);
        }
    }
}
