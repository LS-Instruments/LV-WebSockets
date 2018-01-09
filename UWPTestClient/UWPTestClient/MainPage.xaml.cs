using System;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPTestClient
{
    public partial class MainPage : INotifyPropertyChanged
    {
        private ClientWebSocket _myWebSocket;
        private bool _connected;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool Connected
        {
            get => _connected;
            set
            {
                _connected = value;
                OnPropertyChanged("Connected");
                OnPropertyChanged("IsNotConnected");
            }
        }

        public bool IsNotConnected => !_connected;

        private async Task OpenSocketConnection()
        {
            try
            {
                _myWebSocket = new ClientWebSocket();
                var serverUri = new Uri(WebSocketUri.Text);
                var cancellationToken = new CancellationToken();
                await _myWebSocket.ConnectAsync(serverUri, cancellationToken);
                StartReceiveLoop();
                TextBoxChat.Text += $"Ok - Connected! {Environment.NewLine}";
                Connected = true;
            }
            catch (Exception ex)
            {
                Connected = false;
                TextBoxChat.Text += $"Couldn't connect to {WebSocketUri.Text}. Stacktrace:{Environment.NewLine}";
                TextBoxChat.Text += ex.ToString();
            }
        }

        private async void StartReceiveLoop()
        {
            try
            {
                do
                {
                    var rcvBuffer = new ArraySegment<byte>(new byte[1024]);
                    var rcvResult = await _myWebSocket.ReceiveAsync(rcvBuffer, CancellationToken.None);
                    var message = Encoding.UTF8.GetString(rcvBuffer.Array, 0, rcvResult.Count);
                    TextBoxChat.Text += $"(the server said): {message}{Environment.NewLine}";
                } while (_myWebSocket.State == WebSocketState.Open);
            }
            catch (Exception ex)
            {
                TextBoxChat.Text += $"Exception while receiving message. Stacktrace:{Environment.NewLine}";
                TextBoxChat.Text += ex.ToString();
            }
        }

        private async void SendMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = SendText.Text;
                var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await _myWebSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                TextBoxChat.Text += $"(i've sent): {message}{Environment.NewLine}";
            }
            catch (Exception ex)
            {
                TextBoxChat.Text += $"Exception while sending the message. Stacktrace:{Environment.NewLine}";
                TextBoxChat.Text += ex.ToString();
            }
        }

        private void CloseConnectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            _myWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "I'm done with you", CancellationToken.None);
            Connected = false;
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            await OpenSocketConnection();
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SendText_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && Connected)
            {
                SendMessageButton_OnClick(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }
    }
}
