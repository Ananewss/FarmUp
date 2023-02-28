using FarmUp.Model.LineModel;
using FarmUp.Services.Buyer;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FarmUp.Services.LineBot
{
    public class LineBotService
    {
        string _authorizationKey = string.Empty;
        string _lineTypeRegister = string.Empty;
        private readonly ILogger<LineBotService> _logger;
        private readonly IConfiguration _config;

        public LineBotService(ILogger<LineBotService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _authorizationKey = _config["lineBearer"];
        }
        public async Task<bool> LineFlexMessage(string chatId, int FlexType, string TypeLineRegister, Dictionary<string, string> DataDictionary, Dictionary<string, string> DataDictionary2 = null, Dictionary<string, string> DataDictionary3 = null)
        {
            await Task.Delay(1000);
            try
            {

                if (TypeLineRegister.ToLower() == "buyer") _authorizationKey = _config["lineBearer"];
                //else if (TypeLineRegister.ToLower() == "t") _authorizationKey = Environment.GetEnvironmentVariable("Line_Trainer_Token");
                else
                {
                    throw new Exception("TypeLineRegister Not Support!");
                    
                }
                _lineTypeRegister = TypeLineRegister.ToLower();

                var result = false; 

                
                if (FlexType == 1)
                {
                    if (DataDictionary == null)
                    {
                        DataDictionary = new Dictionary<string, string> {
                         { "CreateDate","01/01/2023 00:00"},
                         { "TdpDate","01/01/2023 00:00"},
                         { "ProductType", "หมอนทอง"},
                         { "ProductGrade", "A,B,C"},
                         { "PricePerUnit", "90"},
                         { "location",  "สถานที่"}
                        };
                    }
                    
                    DataDictionary.Add("altText", "ประกาศรับซื้อทุเรียน");
                }
                 
                else if (FlexType == 32)
                {
                    //CustomText   
                    DataDictionary.Add("altText", "CustomText");
                }
                
                result = await CallLine_FlexMessage(chatId, FlexType, DataDictionary, DataDictionary2, DataDictionary3);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<bool> CallLine_FlexMessage(string chatId, int FlexType, Dictionary<string, string> DataDictionary = null, Dictionary<string, string> DataDictionary2 = null, Dictionary<string, string> DataDictionary3 = null)
        {
            var result = false;
            string SendData = null;

            List<LineMessages> listLineMessages = new List<LineMessages>();
            LineMessages lineMessages = new LineMessages();
            lineMessages.type = "flex";
            lineMessages.altText = DataDictionary["altText"];
            lineMessages.contents = LineMessagesContent(FlexType, DataDictionary, DataDictionary2, DataDictionary3);

            var settingsX = new JsonSerializerSettings();
            settingsX.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settingsX.DefaultValueHandling = DefaultValueHandling.Ignore;
            var jsonInput = JsonConvert.SerializeObject(lineMessages.contents, settingsX);

            listLineMessages.Add(lineMessages);




            NoticationLineResult noticationLine = new NoticationLineResult()
            {
                to = chatId,
                messages = listLineMessages
            };

            if (FlexType == 32)
            {
                var customtext = new List<object>();
                customtext.Add(new
                {
                    type = "text",
                    text = DataDictionary["customText"]
                });
                noticationLine.messages = customtext;
            }

            string Type = "/push";
            string PostURL = string.Format("{0}{1}", "https://api.line.me/v2/bot/message", Type);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _authorizationKey);

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            SendData = JsonConvert.SerializeObject(noticationLine, settings);

            var content = new StringContent(SendData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(PostURL, content);

            //HttpResponseMessage response = await client.PostAsync(PostURL, content);
            var reslinexxxxx = JsonConvert.DeserializeObject<object>(content.ReadAsStringAsync().Result);
            var resline = JsonConvert.DeserializeObject<object>(response.Content.ReadAsStringAsync().Result);

            #region LOGFLEX
            //logflex
            //var flexUri = string.Empty;
            //try
            //{
            //    flexUri = DataDictionary["Uri"];
            //}
            //catch (KeyNotFoundException)
            //{
            //    flexUri = null;
            //}
            //var LogFlexData = new LogFlex
            //{
            //    LogFlexID = Guid.NewGuid(),
            //    FlexType = FlexType,
            //    TypeLineRegister = _lineTypeRegister,
            //    LineUserID = chatId,
            //    ContentUri = flexUri,
            //    Content = jsonInput,
            //    StatusCode = (int)response.StatusCode,
            //    ResponseObject = JsonConvert.SerializeObject(resline, settingsX),
            //    CreatedDate = DateTime.Now
            //};
            ////_lineBotRepo = new ILineBotRepository(); 
            //if (chkLineBotIRepo)
            //{
            //    await _lineBotRepo.LogFlexAsync(LogFlexData);
            //}
            //else
            //{
            //    var linebot = new LineBotRepository(null, null);
            //    await linebot.LogFlexAsync(LogFlexData);
            //}
            #endregion


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = true;
            }
            else
            {
                result = false;
                throw new Exception("ส่ง Flex Message ไม่สำเร็จ : " + response.StatusCode.ToString());
            }
            //var resline = JsonConvert.DeserializeObject<object>(response.Content.ReadAsStringAsync().Result); 

            return result;
        }

        private LineContents LineMessagesContent(int FlexType, Dictionary<string, string> DataDictionary, Dictionary<string, string> DataDictionary2, Dictionary<string, string> DataDictionary3)
        {
            //
            LineContents lineContents = new LineContents
            {
                type = "bubble",
                direction = "ltr"
            };

            dynamic dynamicDictionary = DataDictionary;
            dynamic dynamicDictionary2 = DataDictionary2;
            dynamic dynamicDictionary3 = DataDictionary3;

             
            if (FlexType == 1)  
            {
                lineContents.body = LineSetBodyContent("Box_vertical", null);
                lineContents.body.contents.Add(LineSetBodyContent("Header", "ประกาศรับซื้อทุเรียน"));

                lineContents.body.contents.Add(LineSetBodyContent("SubHeader", DataDictionary["CreateDate"]));
                lineContents.body.contents.Add(LineSetBodyContent("Separator", null));
                lineContents.body.contents.Add(LineSetBodyContent("Box_vertical", null));

                lineContents.body.contents[3].contents.Add(LineSetBodyContent("Box_horizontal", null));
                lineContents.body.contents[3].contents[0].contents.Add(LineSetBodyContent("Detail", "สายพันธุ์"));
                lineContents.body.contents[3].contents[0].contents.Add(LineSetBodyContent("Detail_Data", DataDictionary["ProductType"]));

                lineContents.body.contents[3].contents.Add(LineSetBodyContent("Box_horizontal", null));
                lineContents.body.contents[3].contents[1].contents.Add(LineSetBodyContent("Detail", "เกรด"));
                lineContents.body.contents[3].contents[1].contents.Add(LineSetBodyContent("Detail_Data", DataDictionary["ProductGrade"]));

                lineContents.body.contents[3].contents.Add(LineSetBodyContent("Box_horizontal", null));
                lineContents.body.contents[3].contents[2].contents.Add(LineSetBodyContent("Detail", "ราคารับซื้อ / กก."));
                lineContents.body.contents[3].contents[2].contents.Add(LineSetBodyContent("Detail_Data", DataDictionary["PricePerUnit"]));

                lineContents.body.contents[3].contents.Add(LineSetBodyContent("Box_horizontal", null));
                lineContents.body.contents[3].contents[3].contents.Add(LineSetBodyContent("Detail", "วันที่รับซื้อ"));
                lineContents.body.contents[3].contents[3].contents.Add(LineSetBodyContent("Detail_Data", DataDictionary["TdpDate"]));
                 
                lineContents.body.contents[3].contents.Add(LineSetBodyContent("Box_horizontal", null));
                lineContents.body.contents[3].contents[4].contents.Add(LineSetBodyContent("Detail", "สถานที่"));
                lineContents.body.contents[3].contents[4].contents.Add(LineSetBodyContent("Detail_Data", DataDictionary["location"]));

                lineContents.body.contents[3].contents.Add(LineSetBodyContent("Box_horizontal", null));
                lineContents.body.contents[3].contents[5].contents.Add(LineSetBodyContent("Separator", null));

                lineContents.body.contents[3].contents.Add(LineSetBodyContent("Box_horizontal", null));
                lineContents.body.contents[3].contents[6].contents.Add(LineSetBodyContent("Button_ActionText", "สนใจ"));

                //lineContents.footer = LineSetBodyContent("Box_vertical", null);
                //lineContents.footer.contents.Add(LineSetBodyContent("Separator", null));
                //lineContents.footer.contents.Add(LineSetBodyContent("Button_ActionText", "สนใจ"));
            } 
            return lineContents;
        }

        private LineBodyContents LineSetBodyContent(string dataSetContent, string text, string uri = null)
        {
            LineBodyContents result = null;
            if (dataSetContent == "Header")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    weight = "bold",
                    color = "#01A801",
                    size = "md"
                };
            }
            else if (dataSetContent == "Header_Danger")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    weight = "bold",
                    color = "#E6294D",
                    size = "md"
                };
            }
            else if (dataSetContent == "Header_Info")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    weight = "bold",
                    color = "#69C9C9",
                    size = "md"
                };
            }
            else if (dataSetContent == "Header_Payment_1")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    weight = "bold",
                    color = "#000000",
                    size = "md",
                    align = "center"
                };
            }
            else if (dataSetContent == "Header_Payment_2")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    weight = "bold",
                    color = "#000000",
                    size = "sm",
                    align = "center"
                };
            }
            else if (dataSetContent == "Header_Payment_3")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    weight = "bold",
                    color = "#01A801",
                    size = "md",
                    align = "center"
                };
            }
            else if (dataSetContent == "Header_Payment_4")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    weight = "bold",
                    color = "#000000",
                    size = "xs",
                    align = "center"
                };
            }
            else if (dataSetContent == "SubHeader")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    size = "sm",
                    margin = "md"
                };
            }
            else if (dataSetContent == "Separator")
            {
                result = new LineBodyContents
                {
                    type = "separator",
                    margin = "xxl"
                };
            }
            else if (dataSetContent == "Separator_Danger")
            {
                result = new LineBodyContents
                {
                    type = "separator",
                    margin = "xxl",
                    color = "#E6294D"

                };
            }
            else if (dataSetContent == "Detail")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#555555",
                    size = "sm",
                    flex = 0
                };
            }
            else if (dataSetContent == "Detail_Data")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#111111",
                    size = "sm",
                    align = "end"
                };
            }
            else if (dataSetContent == "Detail_Data_Bold")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#111111",
                    size = "sm",
                    align = "end",
                    weight = "bold"
                };
            }
            else if (dataSetContent == "Detail_Data_status")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#FF9900",
                    size = "sm",
                    align = "end"
                };
            }
            else if (dataSetContent == "Detail_Data_total")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#01A801",
                    weight = "bold",
                    size = "md",
                    align = "end"
                };
            }
            else if (dataSetContent == "Box_vertical")
            {
                result = new LineBodyContents
                {
                    type = "box",
                    layout = "vertical",
                    margin = "xxl",
                    spacing = "sm",
                    contents = new List<LineBodyContents>()
                };
            }
            else if (dataSetContent == "Box_horizontal")
            {
                result = new LineBodyContents
                {
                    type = "box",
                    layout = "horizontal",
                    contents = new List<LineBodyContents>()
                };
            }
            else if (dataSetContent == "Button")
            {
                result = new LineBodyContents
                {
                    type = "button",
                    style = "primary",
                    color = "#01A801",
                    height = "sm",
                    action = new LineBodyContents { type = "uri", label = text, uri = uri }
                };
            }
            else if (dataSetContent == "Button_Danger")
            {
                result = new LineBodyContents
                {
                    type = "button",
                    style = "primary",
                    color = "#E6294D",
                    height = "sm",
                    action = new LineBodyContents { type = "uri", label = text, uri = uri }
                };
            }
            else if (dataSetContent == "Button_Info")
            {
                result = new LineBodyContents
                {
                    type = "button",
                    style = "primary",
                    color = "#69C9C9",
                    height = "sm",
                    action = new LineBodyContents { type = "uri", label = text, uri = uri }
                };
            }
            else if (dataSetContent == "Button_Success")
            {
                result = new LineBodyContents
                {
                    type = "button",
                    style = "primary",
                    color = "#75c626",
                    height = "sm",
                    action = new LineBodyContents { type = "uri", label = text, uri = uri }
                };
            }
            else if (dataSetContent == "Button_ActionText")
            {
                result = new LineBodyContents
                {
                    type = "button",
                    style = "primary",
                    color = "#75c626",
                    height = "sm",
                    action = new LineBodyContents { type = "message", label = text, text = text }
                };
            }
            else if (dataSetContent == "ProfileApprove")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#1DB446",
                    weight = "bold",
                    size = "sm",
                    align = "center"
                };
            }
            else if (dataSetContent == "ProfileReject")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#FF2D00",
                    weight = "bold",
                    size = "sm",
                    align = "center"
                };
            }
            else if (dataSetContent == "Ps")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#BFBBBB",
                    size = "sm",
                    align = "center"
                };
            }
            else if (dataSetContent == "Ps_Danger")
            {
                result = new LineBodyContents
                {
                    type = "text",
                    text = text,
                    color = "#E6294D",
                    size = "md",
                    align = "center",
                    margin = "none"
                };
            }
            else if (dataSetContent == "Image_Center")
            {
                result = new LineBodyContents
                {
                    type = "image",
                    url = uri,
                    size = "md",
                };
            }

            return result;
        }



    }
}
