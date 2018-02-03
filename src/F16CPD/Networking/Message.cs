using System;

namespace F16CPD.Networking
{
    [Serializable]
    public class Message
    {
        public Message()
        {
        }

        public Message(string messageType, object payload) : this()
        {
            MessageType = messageType;
            Payload = payload;
        }

        public string MessageType { get; set; }
        public object Payload { get; set; }
    }
}