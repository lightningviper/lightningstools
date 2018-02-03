using System;

namespace MFDExtractor.Networking
{
    [Serializable]
    public class Message
    {
        public Message() { }
        public Message(MessageType messageType, object payload =null):this()
        {
            MessageType = messageType;
            Payload = payload;
        }

        public MessageType MessageType { get; set; }
        public object Payload { get; set; }
    }
}