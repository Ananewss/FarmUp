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

        public async Task<WeatherForecastDtoList> GetWeatherForecastByLocation(string lineUserId)
        {
            WeatherForecastDtoList weatherForecastDtoList = new WeatherForecastDtoList();
            string strConn = _config.GetConnectionString($"onedurian");

            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            try
            {
                await mSqlConn.OpenAsync();

                MySqlCommand usrCmd = new MySqlCommand(@"SELECT slr.slr_district,slr.slr_province,slr.slr_country
                                                        FROM ma_user
                                                        LEFT JOIN ma_seller slr on ma_user.usr_id = slr.slr_usr_id
                                                        WHERE usr_line_id = @lineUserId",mSqlConn);
                usrCmd.Parameters.AddWithValue("@lineUserId", lineUserId);

                String District = "";
                String Province = "";
                var readDataUsr = await usrCmd.ExecuteReaderAsync();
                while (await readDataUsr.ReadAsync())
                {
                    District = readDataUsr["slr_district"].ToString() ?? "";
                    Province = readDataUsr["slr_province"].ToString() ?? "";
                }
                readDataUsr.Close();
                readDataUsr.Dispose();

                StringBuilder sbReadWeather = new StringBuilder();
                sbReadWeather.Append($"SELECT * FROM tr_weather_day ");
                sbReadWeather.Append($"WHERE District IN (@District) ");
                sbReadWeather.Append($"AND Province IN (@Province) ");
                sbReadWeather.Append($"AND DATE(DT) IN (CURDATE(), CURDATE() + INTERVAL 1 DAY) ");
                sbReadWeather.Append($"ORDER BY DT");
                _logger.LogInformation($"[WeatherForecastService][GetWeatherForecastByLocation][sbReadWeather] = {sbReadWeather.ToString()}");

                MySqlCommand mCmd = new MySqlCommand(sbReadWeather.ToString(), mSqlConn);
                mCmd.Parameters.AddWithValue("@District",District);
                mCmd.Parameters.AddWithValue("@Province", Province);

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
                readData.Close();
                readData.Dispose();

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
