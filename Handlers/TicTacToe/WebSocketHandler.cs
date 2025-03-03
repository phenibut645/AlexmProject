using alexm_app.Models.TicTacToe;
using alexm_app.Models.TicTacToe.ClientMessages.WebSocket;
using alexm_app.Models.TicTacToe.ServerMessages;
using alexm_app.Models.TicTacToe.ServerMessages.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace alexm_app.Utils.TicTacToe
{
    public delegate void PlayerConnectedDelegate(PlayerConnected message);
    public delegate void MessageFailedDelegate();
    public delegate void ConnectionCompletedDelegate(ConnectionCompleted message);
    public delegate void PlayerMovedDelegate(PlayerMoved message);
    public delegate void PlayerReconnectedDelegate();
    public delegate void WebSocketCloseDelegate();
    public delegate void PlayerDisconnectedDelegate();
    public static class WebSocketHandler
    {
        public static event PlayerConnectedDelegate OnPlayerConnect;
        public static event MessageFailedDelegate OnMessageFail;
        public static event ConnectionCompletedDelegate OnConnectionComplete;
        public static event PlayerMovedDelegate OnPlayerMove;
        public static event PlayerReconnectedDelegate OnPlayerReconnect;
        public static event WebSocketCloseDelegate OnWebSocketClose;
        public static event Action OnPlayerWin;
        public static event Action OnPlayerDisconnect;
        public static event Action OnDraw;
        
        public static List<ClientMessage> OnReadyMessages { get; private set; } = new List<ClientMessage>();
        private static readonly Uri ServerUri  = new Uri(@"ws://api.aleksandermilisenko23.thkit.ee");
        private static ClientWebSocket _webSocket = new ClientWebSocket();
        private static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private static CancellationToken Token = CancellationTokenSource.Token;

        public static WebSocketState GetWebSocketState()
        {
            return _webSocket.State;
        }
        public static async Task Connect()
        {
            try
            {
                CancellationTokenSource = new CancellationTokenSource();
                Token = CancellationTokenSource.Token;

                await _webSocket.ConnectAsync(ServerUri, Token);
                
                var buffer = new byte[1024];

                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                dynamic? message = JsonConvert.DeserializeObject(receivedMessage);
                if(message != null && message?.message_type != null)
                {
                    if(message.message_type == "ConnectedToWebSocket")
                    {
                        Debug.WriteLine("Connected to web socket.");
                        foreach(ClientMessage clientMessage in OnReadyMessages)
                        {
                            Debug.WriteLine(JsonConvert.SerializeObject(clientMessage));
                            await SendMessage(clientMessage);
                        } 
                        OnReadyMessages.Clear();
                            

                        Debug.WriteLine($"Adding listener for messages... {_webSocket.State}");
                        Task task = Task.Run(() => ListenForMessagesAsync(), Token);
                        Debug.WriteLine("Listener has been added for messages.");
                    }
                }

                
            }
            catch(WebSocketException ex)
            {
                Debug.WriteLine("WebSocket Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("General Exception: " + ex.Message);
            }
        }
        public static async Task Close()
        {
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", Token);
                CancellationTokenSource.Cancel();
                _webSocket = new ClientWebSocket();
            }
            catch (WebSocketException ex)
            { 
                Debug.WriteLine(ex);
            }
        }
        private static async Task ListenForMessagesAsync()
        {
            var buffer = new byte[1024];
            while (!Token.IsCancellationRequested || _webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), Token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Debug.WriteLine(json);
                        await HandleMessage(json);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error in receiving message: " + ex.Message);
                }
            }
            if(_webSocket.State != WebSocketState.Open)
            {
                OnWebSocketClose?.Invoke();
                Debug.WriteLine("Socket is close, current state is: " + _webSocket.State);
            }
        }

        private static async Task HandleMessage(string json)
        {
            try
            {
                ServerMessagee message = JsonConvert.DeserializeObject<ServerMessagee>(json);
                if(message != null)
                {
                    Debug.WriteLine(message.MessageType + " ---------------------------------------------------------------------");
                    switch (message.MessageType)
                    {
                        case "ConnectionCompleted":
                            ConnectionCompleted connectionCompleted = JsonConvert.DeserializeObject<ConnectionCompleted>(json);
                            OnConnectionComplete?.Invoke(connectionCompleted);
                            break;
                        case "MessageFailed":
                            MessageFailed messageFailed = JsonConvert.DeserializeObject<MessageFailed>(json);
                            OnMessageFail?.Invoke();
                            break;
                        case "PlayerConnected":
                            PlayerConnected playerConnected = JsonConvert.DeserializeObject<PlayerConnected>(json);
                            Debug.WriteLine($"Player connected: {playerConnected.PlayerUsername}");
                            OnPlayerConnect?.Invoke(playerConnected);
                            break;
                        case "PlayerMoved":
                            PlayerMoved playerMoved = JsonConvert.DeserializeObject<PlayerMoved>(json);
                            OnPlayerMove?.Invoke(playerMoved);
                            break;
                        case "PlayerReconnected":
                            PlayerReconnected playerReconnected = JsonConvert.DeserializeObject<PlayerReconnected>(json);
                            OnPlayerReconnect?.Invoke();
                            break;
                        case "PlayerWon":
                            Models.TicTacToe.ServerMessages.WebSocket.PlayerWon playerWon = JsonConvert.DeserializeObject<Models.TicTacToe.ServerMessages.WebSocket.PlayerWon>(json);
                            OnPlayerWin?.Invoke();
                            break;
                        case "PlayerDisconnected":
                            OnPlayerDisconnect?.Invoke();
                            break;
                        case "Draw":
                            OnDraw?.Invoke();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("problem with message handling, json " + json);
                Debug.WriteLine(e);
            }

        }
        public static async Task SendMessage<T>(T message) where T: ClientMessage
        {
            try
            {
                string json = JsonConvert.SerializeObject(message);
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
                await _webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch(WebSocketException ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
