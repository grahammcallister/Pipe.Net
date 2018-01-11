using System;
using System.ComponentModel;
using System.Windows;
using Pipe.Demo.Annotations;

namespace Pipe.Demo
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private PipeClient _client;
        private string _client1OutputText;
        private bool _isClient1Connected;
        private bool _isServerConnected;
        private PipeServer _server;
        private string _serverOutputText;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SetupServer();
            SetupClient();
        }

        #region Server

        private void SetupServer()
        {
            _server = new PipeServer();
            _server.Connect += ServerOnConnect;
            _server.Disconnect += ServerOnDisconnect;
            _server.MessageReceived += ServerOnMessageReceived;
            ServerCloseButton.IsEnabled = false;
        }

        private void DestroyServer()
        {
            _server.Connect -= ServerOnConnect;
            _server.Disconnect -= ServerOnDisconnect;
            _server.MessageReceived -= ServerOnMessageReceived;
            _server = null;
            ServerCloseButton.IsEnabled = false;
        }

        public bool IsServerConnected
        {
            get => _isServerConnected;
            set
            {
                if (value == _isServerConnected) return;
                _isServerConnected = value;
                OnPropertyChanged(nameof(IsServerConnected));
            }
        }

        public string ServerOutputText
        {
            get => _serverOutputText;
            set
            {
                if (value == _serverOutputText) return;
                _serverOutputText = value;
                OnPropertyChanged(nameof(ServerOutputText));
            }
        }

        private void ServerOnConnect(object sender, EventArgs args)
        {
            IsServerConnected = true;
            ServerOutputText += $"Server connect\r\n";
        }

        private void ServerOnDisconnect(object sender, EventArgs args)
        {
            IsServerConnected = false;
            ServerOutputText += $"Server disconnect\r\n";
        }

        private void ServerOnMessageReceived(object sender, MessageEventArgs args)
        {
            if (args != null)
            {
                var message = args.Message;
                ServerOutputText += $"Received from client: {message}";
            }
        }

        private void Server_Open_Button_Click(object sender, RoutedEventArgs e)
        {
            ServerOpenButton.IsEnabled = false;
            _server.Open();
            ServerCloseButton.IsEnabled = true;
            ServerOutputText += $"Server open\r\n";
        }

        private void Server_Close_Button_Click(object sender, RoutedEventArgs e)
        {
            ServerCloseButton.IsEnabled = false;
            _server.Close();
            ServerOpenButton.IsEnabled = true;
            ServerOutputText += $"Server close\r\n";
        }

        private void ServerRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ServerCloseButton.IsEnabled = false;
            _server.Close();
            DestroyServer();
            SetupServer();
            ServerOpenButton.IsEnabled = true;
        }

        private void SendToClientsButton_Click(object sender, RoutedEventArgs e)
        {
            _server.SendMessage(SendToClientsTextBox.Text);
        }

        #endregion Server

        #region Client
        private void SetupClient()
        {
            _client = new PipeClient();
            _client.Connect += Client1OnConnect;
            _client.Disconnect += Client1OnDisconnect;
            _client.MessageReceived += ClientOnMessageReceived;
            ClientCloseButton.IsEnabled = false;
        }

        private void DestroyClient()
        {
            _client.Connect -= Client1OnConnect;
            _client.Disconnect -= Client1OnDisconnect;
            _client.MessageReceived -= ClientOnMessageReceived;
            _client = null;
            ClientCloseButton.IsEnabled = false;
        }

        public string Client1OutputText
        {
            get => _client1OutputText;
            set
            {
                if (value == _client1OutputText) return;
                _client1OutputText = value;
                OnPropertyChanged(nameof(Client1OutputText));
            }
        }

        public bool IsClient1Connected
        {
            get => _isClient1Connected;
            set
            {
                if (value == _isClient1Connected) return;
                _isClient1Connected = value;
                OnPropertyChanged(nameof(IsClient1Connected));
            }
        }

        private void Client1OnConnect(object sender, EventArgs args)
        {
            IsClient1Connected = true;
            Client1OutputText += "Client 1 connect\r\n";
        }

        private void Client1OnDisconnect(object sender, EventArgs args)
        {
            IsClient1Connected = false;
            Client1OutputText += "Client 1 disconnect\r\n";
        }

        private void ClientOnMessageReceived(object sender, MessageEventArgs args)
        {
            if (args != null)
            {
                var message = args.Message;
                Client1OutputText += $"Received from server: {message}";
            }
        }

        private void Client1_Open_Button_Click(object sender, RoutedEventArgs e)
        {
            ClientOpenButton.IsEnabled = false;
            _client.Open();
            ClientCloseButton.IsEnabled = true;
            Client1OutputText += "Client 1 open\r\n";
        }

        private void Client1_Close_Button_Click(object sender, RoutedEventArgs e)
        {
            ClientCloseButton.IsEnabled = false;
            _client.Close();
            ClientOpenButton.IsEnabled = true;
            Client1OutputText += "Client 1 close\r\n";
        }

        private void Client1RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ClientCloseButton.IsEnabled = false;
            _client.Close();
            DestroyClient();
            SetupClient();
            ClientOpenButton.IsEnabled = true;
        }

        private void SendToServerButton_Click(object sender, RoutedEventArgs e)
        {
            _client.SendMessage(SendToServerTextBox.Text);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}