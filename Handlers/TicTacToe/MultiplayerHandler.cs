using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alexm_app.Enums.TicTacToe;
using alexm_app.Models.TicTacToe;
using alexm_app.Models.TicTacToe.ClientMessages.WebSocket;
using alexm_app.Models.TicTacToe.ServerMessages;
using alexm_app.Controls;
using alexm_app.Utils.TicTacToe;
namespace alexm_app.Services
{
    public static class MultiplayerHandler
    {
        public static GameConnection Connection;
        public static bool IsGameRunning;
        public static TicTacToePage CurrentGamePage;
        public static Player? EnemyPlayer = null;
        public static Player? CurrentPlayer = null;

        public static Sides FirstSideMove = Sides.Cross;
        private static Sides _currentSideMove = FirstSideMove;
        public static Sides CurrentSideMove
        {
            get { return _currentSideMove; }
            set
            {
                _currentSideMove = value;
                UpdateSideText();
            }
        }

        static MultiplayerHandler()
        {
            WebSocketHandler.OnPlayerWin += WebSocketHandler_OnPlayerWin;
            WebSocketHandler.OnPlayerConnect += WebSocketHandler_OnPlayerConnect;
            WebSocketHandler.OnPlayerMove += WebSocketHandler_OnPlayerMove;
            WebSocketHandler.OnPlayerReconnect += WebSocketHandler_OnPlayerReconnect;
            WebSocketHandler.OnWebSocketClose += WebSocketHandler_OnWebSocketClose;
            WebSocketHandler.OnConnectionComplete += WebSocketHandler_OnConnectionComplete;
        }

        private static void WebSocketHandler_OnConnectionComplete(Models.TicTacToe.ServerMessages.WebSocket.ConnectionCompleted message)
        {
            if(EnemyPlayer != null && CurrentPlayer != null)
            {
                CurrentGamePage.ServerState.Text = $"Connection completed! Your enemy is: {EnemyPlayer.Username}";
                CurrentPlayer.Side = GetSide(message.Turn);
                EnemyPlayer.Side = GetSide(!message.Turn);

                UpdateSideText();
            }
        }

        private static void WebSocketHandler_OnWebSocketClose()
        {
            
        }

        private static void WebSocketHandler_OnPlayerReconnect()
        {
            
        }

        private static void WebSocketHandler_OnPlayerMove(Models.TicTacToe.ServerMessages.WebSocket.PlayerMoved message)
        {
            if(EnemyPlayer != null && EnemyPlayer.Side != null && CurrentPlayer != null && CurrentPlayer.Side != null)
            {
                CellButton cell = CurrentGamePage.GetCellButton(message.X, message.Y);
                cell.Closed = true;
                CurrentGamePage.ColourCell((Sides)EnemyPlayer.Side, true, cell);
                CurrentSideMove = (Sides)CurrentPlayer.Side;
            }
        }

        private static void WebSocketHandler_OnPlayerConnect(Models.TicTacToe.ServerMessages.WebSocket.PlayerConnected message)
        {
            if(CurrentPlayer != null)
            {
                EnemyPlayer = new Player(message.PlayerUsername);

                CurrentPlayer.Side = GetSide(message.Turn);
                EnemyPlayer.Side = GetSide(!message.Turn);
                CurrentGamePage.ServerState.Text = $"{EnemyPlayer.Username} connected";

                UpdateSideText();
            }
        }

        public static void UpdateSideText()
        {
            if(CurrentGamePage != null && CurrentPlayer != null)
            {
                CurrentGamePage.CurrentSide.Text = $"Current side: {CurrentSideMove}";
                CurrentGamePage.PlayerSide.Text = $"Your side: {CurrentPlayer.Side}";
            }
        }

        private static void WebSocketHandler_OnPlayerWin()
        {
            
        }

        public static Sides GetSide(bool value)
        {
            return value ? Sides.Cross : Sides.Nought;
        }

        public static async Task JoinPlayer(string username, AvailableGame game)
        {
            Connection = GameConnection.Connect;

            EnemyPlayer = new Player(game.Username) { Id = game.PlayerId };
            CurrentPlayer = new Player("username");
            WebSocketHandler.OnReadyMessages.Add(new InitialJoinMessage() { RoomName = game.RoomName, Username = username });
            CurrentGamePage = new TicTacToePage(PageCreated);
            CurrentGamePage.ServerState.Text = $"Connection to {EnemyPlayer.Username}...";
            await GameStateService.Navigation.PushAsync(CurrentGamePage);
        }
        public static async Task CreateRoom(string username, string room)
        {
            Connection = GameConnection.Create;
            CurrentPlayer = new Player("username");
            WebSocketHandler.OnReadyMessages.Add(new InitialCreateMessage() { RoomName = room, Username = username });
            CurrentGamePage = new TicTacToePage(PageCreated);
            await GameStateService.Navigation.PushAsync(CurrentGamePage);
        }
        public static async Task PageCreated()
        {
            AddPlayerEventListeners();
            await WebSocketHandler.Connect();
        }
        public static void AddPlayerEventListeners()
        {
            if(CurrentGamePage != null)
            {
                CurrentGamePage.OnCellClick += CurrentGamePage_OnCellClick;
                CurrentGamePage.OnGameAreaCreate += CurrentGamePage_OnGameAreaCreate;
            }
        }

        private static void CurrentGamePage_OnGameAreaCreate()
        {
            
        }

        private static async void CurrentGamePage_OnCellClick(CellButton cell)
        {
            if(WebSocketHandler.GetWebSocketState() == System.Net.WebSockets.WebSocketState.Open)
            {
                if(CurrentPlayer != null && CurrentSideMove == CurrentPlayer.Side && !cell.Closed && cell.CellInGameArea != null && CurrentPlayer.Side != null)
                {
                    CurrentGamePage.ColourCell((Sides)CurrentPlayer.Side, false, cell);
                    cell.Closed = true;
                    CurrentSideMove = (Sides)EnemyPlayer.Side;
                    await WebSocketHandler.SendMessage(new PlayerMove() { X = cell.CellInGameArea.X , Y = cell.CellInGameArea.Y });
                }
            }
        }
    }
}
