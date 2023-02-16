﻿using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineNoti
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Dictionary<string, object> config;

        private void Form1_Load(object sender, EventArgs e)
        {
            config = Util.XmlReader.Read("config.xml");

            //10 mins
            timer1.Interval = 10 * 60 * 1000;
            timer1.Enabled = true;

            timer1_Tick(sender,null);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            if((now.Hour == 5 && now.Minute >= 25) ||
                (now.Hour == 11 && now.Minute >= 25) ||
                (now.Hour == 17 && now.Minute >= 25) ||
                (now.Hour == 23 && now.Minute >= 25))
            {
                GenMessage(true);
            }
            else if(now.Hour == 2)
            {
                ClearDatabaseTrash();
            }
        }

        private void ClearDatabaseTrash()
        {
            var connstr = config["DbConnectionString"].ToString();
            var conn = new MySql.Data.MySqlClient.MySqlConnection(connstr);

            try
            {
                conn.Open();

                {
                    MySqlCommand cmd = new MySqlCommand(@"DELETE FROM tr_weather_day WHERE DT < DATE_SUB(CURDATE(), INTERVAL 2 DAY)", conn);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch { }
        }

        private void GenMessage(bool sendLine)
        {
            var connstr = config["DbConnectionString"].ToString();
            var conn = new MySql.Data.MySqlClient.MySqlConnection(connstr);

            try
            {
                conn.Open();

                List<Location> listLoc = new List<Location>();
                {
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"SELECT slr.*, usr.usr_line_id
                        FROM ma_seller slr
                        LEFT JOIN ma_user usr ON slr.slr_usr_id = usr.usr_id
                        WHERE slr_dt_weather_noti IS NULL OR DATE_FORMAT(slr_dt_weather_noti, '%Y-%m-%d %H') <> DATE_FORMAT(NOW(), '%Y-%m-%d %H') AND usr.usr_line_id IS NOT NULL AND usr.usr_line_id <> ''
                        ORDER BY slr_district,slr_province,slr_country";

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Location loc = new Location();
                        loc.slr_id = reader["slr_id"].ToString();
                        loc.lineUserId = reader["usr_line_id"].ToString();
                        loc.District = reader["slr_district"].ToString();
                        loc.Province = reader["slr_province"].ToString();
                        loc.Country = reader["slr_country"].ToString();
                        listLoc.Add(loc);
                    }
                    reader.Close();
                }

                bool isCloudy = false;
                bool isRain = false;
                bool isHeavyRain = false;
                {
                    foreach (var loc in listLoc)
                    {
                        MySqlCommand cmd = conn.CreateCommand();
                        cmd.CommandText = @"SELECT *
                                FROM tr_weather_day
                                WHERE DT >= DATE_FORMAT(NOW(),'%Y-%m-%d %H:00:00') AND DT < DATE_FORMAT(NOW(),'%Y-%m-%d 23:59:59')
                                    AND District=@District
                                    AND Province=@Province
                                    AND Country=@Country
                                ORDER BY DT
                                    ";

                        cmd.Parameters.AddWithValue("@District", loc.District);
                        cmd.Parameters.AddWithValue("@Province", loc.Province);
                        cmd.Parameters.AddWithValue("@Country", loc.Country);

                        List<RainFromTo> rainFromTos = new List<RainFromTo>();
                        var reader = cmd.ExecuteReader();
                        bool rainStart = false;
                        RainFromTo current = null;

                        while (reader.Read())
                        {
                            var title = reader["Title"].ToString().ToLower();
                            //if (Int32.Parse(reader["rainAmt"].ToString()) >= 30)
                            if (title.IndexOf("rain") >= 0)
                            {
                                isRain = true;
                                if (!rainStart)
                                {
                                    rainStart = true;
                                    current = new RainFromTo();
                                    current.from = ((DateTime)(reader["DT"])).ToString("HH:00");
                                }
                                else
                                {
                                    current.to = ((DateTime)(reader["DT"])).ToString("HH:00");
                                }
                            }
                            else
                            {
                                rainStart = false;
                                if (current != null)
                                {
                                    rainFromTos.Add(current);
                                    current = null;
                                }
                            }


                            if (title.IndexOf("cloudy") >= 0)
                                isCloudy = true;

                            if (title.IndexOf("heavy rain") >= 0)
                                isHeavyRain = true;
                        }
                        if (current != null)
                        {
                            current.to = "24:00";
                            rainFromTos.Add(current);
                            current = null;
                        }
                        reader.Close();

                        //analyse message
                        if (rainFromTos.Count == 0)
                            loc.Message = "สวัสดี วันนี้แจ่มใสทั้งวันโอกาสฝนตกน้อยมาก";
                        else
                        {
                            loc.Message = "สวัสดี วันนี้มีโอกาสที่ฝนจะตกช่วง ";
                            bool printAnd = false;
                            foreach (var fromto in rainFromTos)
                            {
                                if (!printAnd)
                                    printAnd = true;
                                else
                                    loc.Message += " และ ";
                                loc.Message += fromto.from;
                                if (!String.IsNullOrEmpty(fromto.to))
                                    loc.Message += "-" + fromto.to;
                            }
                        }
                    }

                    //select picture
                    var message = "อากาศวันนี้แจ่มใส";
                    var img = "clearskyPic.jpg";
                    if (isHeavyRain)
                    {
                        img = "stormPic.jpg";
                        message = "อากาศวันนี้ฝนตกหนัก";
                    }
                    else if (isRain)
                    {
                        img = "rainPic.jpg";
                        message = "อากาศวันนี้ฝนตก";
                    }
                    else if (isCloudy)
                    {
                        img = "cloudPic.jpg";
                        message = "อากาศวันนี้เมฆ";
                    }

                    if(sendLine)
                        //Send Line
                        foreach (var loc in listLoc)
                        {
                            var FullTemplate = @"{
  ""to"": ""==to=="",
  ""messages"": [
    {
      ""type"": ""flex"",
      ""altText"": """+ message + @""",
      ""contents"": {
        ""type"": ""bubble"",
        ""header"": {
          ""type"": ""box"",
          ""layout"": ""vertical"",
          ""contents"": [
            {
              ""type"": ""box"",
              ""layout"": ""horizontal"",
              ""contents"": [
                {
                  ""type"": ""box"",
                  ""layout"": ""vertical"",
                  ""contents"": [
                    {
                      ""type"": ""image"",
                      ""url"": ""https://queenbeautynetwork.com/images/"+ img + @""",
                      ""size"": ""full"",
                      ""aspectMode"": ""cover"",
                      ""aspectRatio"": ""150:98"",
                      ""gravity"": ""center""
                    }
                  ],
                  ""flex"": 1
                }
              ]
            }
          ],
          ""paddingAll"": ""0px""
        },
        ""body"": {
          ""type"": ""box"",
          ""layout"": ""vertical"",
          ""contents"": [
            {
              ""type"": ""box"",
              ""layout"": ""vertical"",
              ""contents"": [
                {
                  ""type"": ""box"",
                  ""layout"": ""vertical"",
                  ""contents"": [
                    {
                      ""type"": ""text"",
                      ""contents"": [],
                      ""size"": ""xl"",
                      ""wrap"": true,
                      ""text"": ""==Location=="",
                      ""color"": ""#ffffff"",
                      ""weight"": ""bold""
                    },
                    {
                      ""type"": ""text"",
                      ""text"": ""==Date=="",
                      ""color"": ""#ffffffcc"",
                      ""size"": ""sm""
                    }
                  ],
                  ""spacing"": ""sm""
                },
                {
                  ""type"": ""box"",
                  ""layout"": ""vertical"",
                  ""contents"": [
                    {
                      ""type"": ""box"",
                      ""layout"": ""vertical"",
                      ""contents"": [
                        {
                          ""type"": ""text"",
                          ""contents"": [],
                          ""size"": ""sm"",
                          ""wrap"": true,
                          ""margin"": ""lg"",
                          ""color"": ""#ffffffde"",
                          ""text"": ""==WeatherMessage==""
                        }
                      ]
                    }
                  ],
                  ""paddingAll"": ""13px"",
                  ""backgroundColor"": ""#ffffff1A"",
                  ""cornerRadius"": ""2px"",
                  ""margin"": ""xl""
                }
              ]
            },
            {
              ""type"": ""button"",
              ""action"": {
                ""type"": ""uri"",
                ""label"": ""ดูรายละเอียด"",
                ""uri"": ""https://queenbeautynetwork.com/?ref=Seller-WeatherForecast""
              },
              ""color"": ""#ffffff"",
              ""style"": ""secondary"",
              ""margin"": ""10px""
            }
          ],
          ""paddingAll"": ""20px"",
          ""backgroundColor"": ""#464F69""
        }
      }
    }
  ]
}";

                            FullTemplate = FullTemplate.Replace("==to==", loc.lineUserId.Trim());
                            FullTemplate = FullTemplate.Replace("==Location==", loc.District);
                            FullTemplate = FullTemplate.Replace("==Date==", DateTime.Now.ToString("dd/MM/yyyy"));
                            FullTemplate = FullTemplate.Replace("==WeatherMessage==", loc.Message);

                            var message1 = new Message("text", loc.Message);
                            var root = new Root();
                            root.to = loc.lineUserId.Trim();
                            root.addMessage(message1);

                            // To serialize
                            var json2 = JsonConvert.SerializeObject(root);
                            var json = FullTemplate;

                            var accessToken = @"gv13S11fDZ2XtAMcz6r3Ar/e9eLjEMAoCB8Ak473flX13fjS6pzfwRyUgF1xziOCxN97Wz4o4j6fcDBi8wutEtv3rUhwg22JX3g0y07/kgyz5nIe9kazcXIZTrgg981vgDlFXVIqdlCxqtvVaoZF1AdB04t89/1O/w1cDnyilFU=";
                            var client = new HttpClient();

                            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/v2/bot/message/push");
                            request.Headers.Add("Authorization", "Bearer " + accessToken);

                            request.Content = new StringContent(
                                json.ToString(),
                                Encoding.UTF8,
                                "application/json"
                            );
                            client.SendAsync(request);
                            //Update Notify Stage

                            MySqlCommand cmd = conn.CreateCommand();
                            cmd.CommandText = @"UPDATE ma_seller SET
                                    slr_dt_weather_noti=DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:00')
                                    WHERE slr_id=@slr_id
                                    ";

                            cmd.Parameters.AddWithValue("@slr_id", loc.slr_id);
                            var reader = cmd.ExecuteNonQuery();

                        }
                    else
                    {
                        foreach (var loc in listLoc)
                        {
                            var msg = String.Format("Send Line[{0}] : {1} \r\n{2}",loc.slr_id,loc.lineUserId,loc.Message);
                            LogStatus(msg);
                        }
                    }
                }
            }
            catch { }
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenMessage(true);
        }
    }
}
