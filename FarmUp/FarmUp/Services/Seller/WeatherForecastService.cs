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
                if (await readDataUsr.ReadAsync())
                {
                    District = readDataUsr["slr_district"].ToString() ?? "";
                    Province = readDataUsr["slr_province"].ToString() ?? "";

                    #region ForTest
                    //District = "Pak Kret";
                    //Province = "Nonthaburi";
                    #endregion Fortest
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

                    var icon = readData["Title"].ToString().ToLower().Replace(" ", "").Trim();
                    icon += (weatherForecastDto.DT.Hour > 18 || weatherForecastDto.DT.Hour < 6)? "_night_V2.png" : "_day_V2.png";
                    weatherForecastDto.Icon = icon;
                    weatherForecastDto.CloudyMsg = (Int32.Parse(weatherForecastDto.CloudCover) >= 60) ? "เมฆมาก" : (Int32.Parse(weatherForecastDto.CloudCover) >= 30) ? "เมฆปานกลาง" : "เมฆน้อย";
                    weatherForecastDto.Title = readData["Title"].ToString() ?? "";
                    switch (weatherForecastDto.Title.ToLower())
                    {
                        case "clear sky": weatherForecastDto.DisplayTitle = "ท้องฟ้าแจ่มใส"; break;
                        case "cloudy": weatherForecastDto.DisplayTitle = "มีเมฆมาก"; break;
                        case "fair": weatherForecastDto.DisplayTitle = "มีเมฆเล็กน้อย"; break;
                        case "fog": weatherForecastDto.DisplayTitle = "มีหมอก"; break;
                        case "heavy rain": weatherForecastDto.DisplayTitle = "ฝนตกหนัก"; break;
                        case "heavy rain and thunder": weatherForecastDto.DisplayTitle = "ฝนตกหนัก"; break;
                        case "heavy rain showers": weatherForecastDto.DisplayTitle = "ฝนตกหนัก"; break;
                        case "heavy rain showers and thunder": weatherForecastDto.DisplayTitle = "ฝนตกหนัก"; break;
                        case "heavy sleet": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "heavy sleet and thunder": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "Heavy sleet showers": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "Heavy sleet showers and thunder": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "light rain": weatherForecastDto.DisplayTitle = "ฝนตกเล็กน้อย"; break;
                        case "light rain and thunder": weatherForecastDto.DisplayTitle = "ฝนตกเล็กน้อย"; break;
                        case "light rain showers": weatherForecastDto.DisplayTitle = "ฝนตกเล็กน้อย"; break;
                        case "light rain showers and thunder": weatherForecastDto.DisplayTitle = "ฝนตกเล็กน้อย"; break;
                        case "light sleet": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "light sleet and thunder": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "light sleet showers": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "light sleet showers and thunder": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "light snow showers and thunder": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "partly cloudy": weatherForecastDto.DisplayTitle = "มีเมฆปานกลาง"; break;
                        case "rain": weatherForecastDto.DisplayTitle = "ฝนตก"; break;
                        case "rain and thunder": weatherForecastDto.DisplayTitle = "ฝนตก"; break;
                        case "rain showers": weatherForecastDto.DisplayTitle = "ฝนตก"; break;
                        case "rain showers and thunder": weatherForecastDto.DisplayTitle = "ฝนตก"; break;
                        case "sleet": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "sleet and thunder": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "sleet showers": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        case "sleet showers and thunder": weatherForecastDto.DisplayTitle = "มีลูกเห็บ"; break;
                        default:
                            weatherForecastDto.DisplayTitle = weatherForecastDto.Title; break;
                    }

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
