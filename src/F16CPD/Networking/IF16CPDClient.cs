namespace F16CPD.Networking
{
    public interface IF16CPDClient
    {
        bool IsConnected { get; }
        object GetSimProperty(string propertyName);
        void SendMessageToServer(Message message);
        void ClearPendingClientMessages();
        Message GetNextPendingClientMessage();
    }
}