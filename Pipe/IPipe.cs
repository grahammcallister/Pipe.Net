namespace Pipe
{
    public interface IPipe
    {
        string PipeName { get; set; }
        bool Connected { get; set; }
        void Open();
        void Close();
        void SendMessage(string message);
        event ConnectEvent Connect;
        event DisconnectEvent Disconnect;
        event MessageEvent MessageReceived;
    }
}