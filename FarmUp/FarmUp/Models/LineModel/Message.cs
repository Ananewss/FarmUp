using Newtonsoft.Json;

namespace FarmUp.Model.LineModel
{
    public class Message
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}