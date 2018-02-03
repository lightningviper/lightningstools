namespace F16CPD.Networking
{
    public interface IF16CPDServer
    {
        object GetSimProperty(string propertyName);
        void SubmitMessageToServer(Message message);
        void ClearPendingClientMessages();
        Message GetNextPendingClientMessage();
        bool TestConnection();
    }
}