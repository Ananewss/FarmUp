using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        ChromeDriver chrome;
        private void button1_Click(object sender, EventArgs e)
        {
            //Bangkok : https://weather.com/weather/hourbyhour/l/22396578e4d85e547ad393f260e37b5d99da89c0a07042eb54eacf3afc2200b8 , Fri Nov 4, 10pm
            GetWeatherForecast("https://weather.com/weather/hourbyhour/l/22396578e4d85e547ad393f260e37b5d99da89c0a07042eb54eacf3afc2200b8");
        }

        private void GetWeatherForecast(string url) {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            chrome = new ChromeDriver(service);
            chrome.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(300);
            chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            chrome.Navigate().GoToUrl(url);

            IJavaScriptExecutor js = chrome as IJavaScriptExecutor;

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
                    catch {
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
                        //var C = ((Int32.Parse(F.Substring(0,F.Length-1)) - 32) * 5) / 9;
                        var percentRain = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "Precip").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "PercentageValue").Value;
                        var wind = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "WindSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "Wind").Value;
                        var humidity = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "HumiditySection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "PercentageValue").Value;
                        var uv = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "uvIndexSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "UVIndexValue").Value;
                        var cloudCover = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "CloudCoverSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "PercentageValue").Value;
                        var rainAmt = data.Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "AccumulationSection").Descendants().FirstOrDefault(x => (string)x.Attribute("data-testid") == "AccumulationValue").Value;

                        LogStatus(String.Format("{0} : {1}*F, Rain-{2}, wind-{3}", fullDt.ToString("d/M/y hh:mm"), F, percentRain, wind));
                    }
                    catch { }
                }
            }

            chrome.Close();

            chrome.Quit();
        }
        public void LogStatus(String data)
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

        private void button2_Click(object sender, EventArgs e)
        {
            

            GetWeatherForecast("https://weather.com/weather/hourbyhour/l/c6eee0f8df89fcfb61c2fd377908f644094974f5ee3fc4f040d3d9038e6ca341");
        }
    }
}
