using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WeatherRobot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        Dictionary<string, object> config;

        private void Form1_Load(object sender, EventArgs e)
        {
            config = Util.XmlReader.Read("config.xml");

            timer1.Interval = Int32.Parse(config["DelayMin"].ToString()) * 60 * 1000;
            timer1.Enabled = true;

            timer2.Interval = Int32.Parse(config["DelayMinUrl"].ToString()) * 60 * 1000; 
            timer2.Enabled = true;

            GetWeatherDayItem();
        }

        ChromeDriver chrome;
        private void button1_Click(object sender, EventArgs e)
        {
            GetWeatherDayItem();
        }

        private void GetWeatherDayItem() {
            var connstr = config["DbConnectionString"].ToString();
            var conn = new MySql.Data.MySqlClient.MySqlConnection(connstr);

            List<ma_location_Item> list = new List<ma_location_Item>();
            try
            {
                conn.Open();

                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * from ma_location";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = new ma_location_Item()
                    {
                        loc_id = reader.GetString("loc_id"),
                        loc_SubDistrict = reader.GetString("loc_SubDistrict"),
                        loc_District = reader.GetString("loc_District"),
                        loc_Province = reader.GetString("loc_Province"),
                        loc_Country = reader.GetString("loc_Country"),
                        loc_weather_url = reader.GetString("loc_weather_url")
                    };

                    if(!String.IsNullOrWhiteSpace(item.loc_weather_url))
                        list.Add(item);
                }

                conn.Clone();
            }
            catch { }

            if(list.Count > 0)
                foreach(var item in list)
                {
                    GetWeatherForecast(item.loc_SubDistrict, item.loc_District, item.loc_Province, item.loc_Country, item.loc_weather_url);
                }
        }

        private void GetWeatherForecast(string subDistrict, string district, string province, string country, string url)
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            chrome = new ChromeDriver(service);
            chrome.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(300);
            chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            chrome.Navigate().GoToUrl("https://weather.com/weather/hourbyhour/l/" + url);

            IJavaScriptExecutor js = chrome as IJavaScriptExecutor;

            List<tr_weather_day_Item> weather_day_list = new List<tr_weather_day_Item>();

            var results = chrome.FindElements(By.XPath("//div[contains(@class,'HourlyForecast')]/*")).ToArray();
            if (js != null)
            {
                DateTime? dt = null;
                for (int i = 0; i < results.Length; i++)
                {
                    try
                    {
                        var ttmp = results[i].Text;
                    }
                    catch
                    {
                    }
                    string innerHtml;
                    try
                    {
                        innerHtml = (string)js.ExecuteScript("return arguments[0].innerHTML;", results[i]);
                    }
                    catch
                    {
                        continue;
                    }
                    try
                    {
                        var txt = innerHtml;
                        dt = DateTime.ParseExact(txt, "dddd, MMMM d", CultureInfo.InvariantCulture);
                        continue;
                    }
                    catch { }

                    if (!innerHtml.Contains("daypartName") && !innerHtml.Contains("detailsTemperature")) continue;

                    try
                    {
                        var data = XElement.Parse("<All>" + innerHtml.Replace("xlink:", "") + "</All>");

                        var time = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "daypartName").Value;
                        var fullDt = DateTime.ParseExact(dt.Value.ToString("d/M/y") + " " + time, "d/M/y h tt", CultureInfo.InvariantCulture);
                        var F = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "detailsTemperature").Value;
                        var C = ((Int32.Parse(F.Substring(0,F.Length-1)) - 32) * 5) / 9;
                        var percentRain = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "Precip").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "PercentageValue").Value;
                        var wind = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "WindSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "Wind").Value;
                        var humidity = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "HumiditySection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "PercentageValue").Value;
                        var uv = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "uvIndexSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "UVIndexValue").Value;
                        var cloudCover = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "CloudCoverSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "PercentageValue").Value;
                        //var rainAmt = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "Precip").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "PercentageValue").Value;
                        var rainAmt = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "AccumulationSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "AccumulationValue").Value;

                        percentRain = percentRain.Replace("%", "");
                        humidity = humidity.Replace("%", "");
                        cloudCover = cloudCover.Replace("%", "");

                        var weather_day = new tr_weather_day_Item()
                        {
                            DT = fullDt,
                            Temp = C.ToString(),
                            Wind = wind,
                            Humidity = humidity,
                            UV = uv,
                            CloudCover = cloudCover,
                            RainAmt = percentRain,
                            SubDistrict = subDistrict,
                            District = district,
                            Province = province,
                            Country = country
                        };
                        weather_day_list.Add(weather_day);
                    }
                    catch { }
                }

                //insert or update database
                if(weather_day_list.Count>0)
                {
                    var connstr = config["DbConnectionString"].ToString();
                    var conn = new MySql.Data.MySqlClient.MySqlConnection(connstr);

                    //Get Location List From Database
                    List<String> locationList = new List<String>();
                    try
                    {
                        conn.Open();

                        foreach (var weather_day in weather_day_list)
                        {
                            bool canRead = false;
                            {
                                MySqlCommand cmd = conn.CreateCommand();
                                cmd.CommandText = "SELECT * from tr_weather_day WHERE DT=@DT AND SubDistrict=@SubDistrict AND District=@District AND Province=@Province AND Country=@Country";
                                cmd.Parameters.AddWithValue("@DT", weather_day.DT);
                                cmd.Parameters.AddWithValue("@SubDistrict", weather_day.SubDistrict);
                                cmd.Parameters.AddWithValue("@District", weather_day.District);
                                cmd.Parameters.AddWithValue("@Province", weather_day.Province);
                                cmd.Parameters.AddWithValue("@Country", weather_day.Country);

                                var reader = cmd.ExecuteReader();
                                canRead = reader.Read();
                                reader.Close();
                            }
                            if (canRead)
                            {
                                MySqlCommand cmd = conn.CreateCommand();
                                cmd.CommandText = "UPDATE tr_weather_day SET Temp=@Temp,Wind=@Wind,Humidity=@Humidity,UV=@UV,CloudCover=@CloudCover,RainAmt=@RainAmt WHERE DT=@DT AND SubDistrict=@SubDistrict AND District=@District AND Province=@Province AND Country=@Country";
                                cmd.Parameters.AddWithValue("@Temp", weather_day.Temp);
                                cmd.Parameters.AddWithValue("@Wind", weather_day.Wind);
                                cmd.Parameters.AddWithValue("@Humidity", weather_day.Humidity);
                                cmd.Parameters.AddWithValue("@UV", weather_day.UV);
                                cmd.Parameters.AddWithValue("@DT", weather_day.DT);
                                cmd.Parameters.AddWithValue("@CloudCover", weather_day.CloudCover);
                                cmd.Parameters.AddWithValue("@RainAmt", weather_day.RainAmt);
                                cmd.Parameters.AddWithValue("@SubDistrict", weather_day.SubDistrict);
                                cmd.Parameters.AddWithValue("@District", weather_day.District);
                                cmd.Parameters.AddWithValue("@Province", weather_day.Province);
                                cmd.Parameters.AddWithValue("@Country", weather_day.Country);

                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                MySqlCommand cmd = conn.CreateCommand();
                                cmd.CommandText = @"INSERT INTO tr_weather_day(Wea_Id,DT,Temp,Wind,Humidity,UV,CloudCover,RainAmt,SubDistrict,District,Province,Country)
                                    VALUES(UNHEX(CONCAT(REPLACE(UUID(), '-', ''))),@DT,@Temp,@Wind,@Humidity,@UV,@CloudCover,@RainAmt,@SubDistrict,@District,@Province,@Country)";
                                cmd.Parameters.AddWithValue("@Temp", weather_day.Temp);
                                cmd.Parameters.AddWithValue("@Wind", weather_day.Wind);
                                cmd.Parameters.AddWithValue("@Humidity", weather_day.Humidity);
                                cmd.Parameters.AddWithValue("@UV", weather_day.UV);
                                cmd.Parameters.AddWithValue("@DT", weather_day.DT);
                                cmd.Parameters.AddWithValue("@CloudCover", weather_day.CloudCover);
                                cmd.Parameters.AddWithValue("@RainAmt", weather_day.RainAmt);
                                cmd.Parameters.AddWithValue("@SubDistrict", weather_day.SubDistrict);
                                cmd.Parameters.AddWithValue("@District", weather_day.District);
                                cmd.Parameters.AddWithValue("@Province", weather_day.Province);
                                cmd.Parameters.AddWithValue("@Country", weather_day.Country);

                                cmd.ExecuteNonQuery();

                            }
                        }
                        conn.Close();
                    }
                    catch (Exception error)
                    {
                        LogStatus("Error READ DB :" + error);
                        this.Close();
                    }
                }
            }

            chrome.Close();

            chrome.Quit();
        }
        public void LogStatus(String data)
        {
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    const int maxLine = 100;

                    richTextBox1.Text = (DateTime.Now.ToUniversalTime().AddHours(7) + "  :  " + data + Environment.NewLine + richTextBox1.Text).Trim();

                    string[] lines = richTextBox1.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > maxLine)
                    {
                        string rs = "";
                        for (int i = 0; i < maxLine; i++)
                            rs += lines[i] + Environment.NewLine;
                        richTextBox1.Text = rs.Trim();
                    }

                    richTextBox1.Refresh();
                });
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetWeatherUrl();
        }

        private void GetWeatherUrl()
        {
            var connstr = config["DbConnectionString"].ToString();
            var conn = new MySql.Data.MySqlClient.MySqlConnection(connstr);

            //Get Location List From Database
            List<String> locationList = new List<String>();
            try
            {
                conn.Open();

                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * from ma_location WHERE loc_weather_url='' OR loc_weather_url IS NULL";
                //cmd.Parameters.AddWithValue("@param", value);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var location = (reader.GetString("loc_District").Equals(reader.GetString("loc_Province"))) ?
                        String.Format("{0}~{1}, {2}", reader.GetString("loc_id"), reader.GetString("loc_Province"), reader.GetString("loc_Country")) :
                        String.Format("{0}~{1}, {2}, {3}", reader.GetString("loc_id"), reader.GetString("loc_District"), reader.GetString("loc_Province"), reader.GetString("loc_Country"));

                    LogStatus(location);
                    locationList.Add(location);
                }
                conn.Close();
            }
            catch (Exception error)
            {
                LogStatus("Error READ DB :" + error);
                this.Close();
            }

            //Get location Code from Weather.com
            Dictionary<string, string> hash = new Dictionary<string, string>();
            if (locationList.Count > 0)
                try
                {
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;

                    chrome = new ChromeDriver(service);
                    chrome.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(300);
                    chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                    chrome.Navigate().GoToUrl("https://weather.com/");

                    foreach (var location in locationList)
                    {
                        var id = location.Substring(0, location.IndexOf('~'));
                        var locationStr = location.Substring(location.IndexOf('~')+1);

                        WaitElement(chrome, By.Id("LocationSearch_input"), 3, true);
                        chrome.FindElement(By.Id("LocationSearch_input")).SendKeys(locationStr);

                        WaitElement(chrome, By.Id("LocationSearch_listbox"), 3);
                        for (int i = 0; i < 10; i++)
                        {
                            var listText = chrome.FindElement(By.Id("LocationSearch_listbox")).Text;
                            if (listText.Contains(locationStr))
                                break;
                            System.Threading.Thread.Sleep(400);
                        }
                        chrome.FindElement(By.Id("LocationSearch_input")).SendKeys(OpenQA.Selenium.Keys.Enter);

                        System.Threading.Thread.Sleep(1000);
                        WaitElement(chrome, By.Id("todayDetails"), 3);
                        for(int i=0; i < 10; i++)
                        {
                            var titleText = chrome.FindElement(By.CssSelector("[data-testid='HeaderTitle'")).Text;
                            if (titleText.Contains(locationStr))
                                break;
                            System.Threading.Thread.Sleep(400);
                        }

                        String currentURL = chrome.Url;
                        var weatherCode = currentURL.Substring(currentURL.LastIndexOf('/') + 1);
                        hash.Add(id, weatherCode);
                    }
                    chrome.Close();
                    chrome.Quit();
                }
                catch (Exception ex)
                {
                }

            //update location Code to database
            try
            {
                conn.Open();

                foreach (KeyValuePair<string, string> kvp in hash)
                {
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE ma_location SET loc_weather_url=@loc_weather_url WHERE loc_id=@loc_id";
                    cmd.Parameters.AddWithValue("@loc_weather_url", kvp.Value);
                    cmd.Parameters.AddWithValue("@loc_id", kvp.Key);

                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                LogStatus("UPDATE DATABASE SUCCESS");
            }
            catch (Exception error)
            {
                LogStatus("Error UPDATE DB :" + error);
                this.Close();
            }

            if (locationList.Count > 0)
            {
                GetWeatherDayItem();
            }
        }

        public static IWebElement WaitElement(IWebDriver driver, By by, int timeoutInSeconds, bool withcheckSendKey = false)
        {
            for(int i = 0; i < timeoutInSeconds * 2; i++)
            {
                try
                {
                    var el = driver.FindElement(by);
                    if (el != null)
                    {
                        if (withcheckSendKey)
                        {
                            el.SendKeys("T");
                            el.SendKeys(OpenQA.Selenium.Keys.Backspace);
                            el.SendKeys(OpenQA.Selenium.Keys.Backspace);
                        }
                        return el;
                    }
                }
                catch {
                    System.Threading.Thread.Sleep(500);
                }
            }
            return null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetWeatherDayItem();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            GetWeatherDayItem();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            GetWeatherUrl();
        }
    }
}

