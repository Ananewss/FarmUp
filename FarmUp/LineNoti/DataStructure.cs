using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineNoti
{
    public class Location
    {
        public String slr_id { get; set; }
        public String lineUserId { get; set; }
        public String District { get; set; }
        public String Province { get; set; }
        public String Country { get; set; }
        public String Message { get; set; }
    }

    public class RainFromTo
    {
        public String from { get; set; }
        public String to { get; set; }
    }


    public class Message
    {
        public string type { get; set; }
        public string text { get; set; }
        public Message(string type, string text)
        {
            this.type = type;
            this.text = text;
        }
    }

    public class Root
    {
        public string to { get; set; }
        public List<Message> messages { get; set; }

        internal void addMessage(Message message1)
        {
            if (messages == null)
                messages = new List<Message>();
            messages.Add(message1);
        }
    }
}
