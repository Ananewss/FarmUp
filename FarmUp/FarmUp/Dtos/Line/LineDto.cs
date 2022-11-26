namespace FarmUp.Dtos.Admin
{
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
            if(messages==null)
                messages = new List<Message>();
            messages.Add(message1);
        }
    }
}

