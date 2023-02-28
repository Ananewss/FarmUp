using System.Collections.Generic;
using Newtonsoft.Json;

namespace FarmUp.Model.LineModel
{
    public class Events
    {
        [JsonProperty("events")]
        public List<EventsList> EventList { get; set; }
    }
}
