﻿using System;
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
        public static event Action OnRunningGameClose;
        public static GameConnection? Connection = null;
        public static bool IsGameRunning = true;
        public static TicTacToePage? CurrentGamePage = null;
        public static Player? EnemyPlayer = null;
        public static Player? CurrentPlayer = null;
        public static bool Win = false;
        public static Sides FirstSideMove = Sides.Cross;
        private static Sides _currentSideMove = FirstSideMove;
        private static int? SizeMap = null;
        public static Sides CurrentSideMove
        {
            get { return _currentSideMove; }
            set
            {
                _currentSideMove = value;
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    UpdateSideText();
                });
                
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
            WebSocketHandler.OnPlayerDisconnect += WebSocketHandler_OnPlayerDisconnect;
            WebSocketHandler.OnDraw += WebSocketHandler_OnDraw;
        }

        private static void WebSocketHandler_OnDraw()
        {
            IsGameRunning = false;
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentGamePage.ServerState.Text = $"Draw!";
            });
        }

        private static void WebSocketHandler_OnPlayerDisconnect()
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentGamePage.ServerState.Text = $"Player has disconnected!";
            });
        }

        private static void WebSocketHandler_OnConnectionComplete(Models.TicTacToe.ServerMessages.WebSocket.ConnectionCompleted message)
        {
            if(EnemyPlayer != null && CurrentPlayer != null)
            {
                IsGameRunning = true;
                CurrentPlayer.Side = GetSide(message.Turn);
                EnemyPlayer.Side = GetSide(!message.Turn);
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CurrentGamePage.ServerState.Text = $"Connection completed! Your enemy is: {EnemyPlayer.Username}";
                    UpdateSideText();
                });
            }
        }

        private static void WebSocketHandler_OnWebSocketClose()
        {
            
        }

        private static void WebSocketHandler_OnPlayerReconnect()
        {
            MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CurrentGamePage.ServerState.Text = $"Connection completed! Your enemy is: {EnemyPlayer.Username}";
                    UpdateSideText();
            });
        }

        private static void WebSocketHandler_OnPlayerMove(Models.TicTacToe.ServerMessages.WebSocket.PlayerMoved message)
        {
            if(EnemyPlayer != null && EnemyPlayer.Side != null && CurrentPlayer != null && CurrentPlayer.Side != null)
            {
                
                CurrentSideMove = (Sides)CurrentPlayer.Side;
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CellButton cell = CurrentGamePage.GetCellButton(message.X, message.Y);
                    cell.Closed = true;
                    cell.Side = EnemyPlayer.Side;
                    CurrentGamePage.ColourCell((Sides)EnemyPlayer.Side, true, cell);
                });
            }
        }

        private static void WebSocketHandler_OnPlayerConnect(Models.TicTacToe.ServerMessages.WebSocket.PlayerConnected message)
        {
            if(CurrentPlayer != null)
            {
                EnemyPlayer = new Player(message.PlayerUsername);
                CurrentPlayer.Side = GetSide(message.Turn);
                EnemyPlayer.Side = GetSide(!message.Turn);
                IsGameRunning = true;
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CurrentGamePage.ServerState.Text = $"{EnemyPlayer.Username} connected";
                    UpdateSideText();
                });
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
            IsGameRunning = false;
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentGamePage.ServerState.Text = $"{EnemyPlayer.Username} won!";
            });
        }
        public static void Rejoin()
        {
            Connection = GameConnection.Connect;
            if(GameStateService.SavedPlayerInfo == null) return;
            CurrentPlayer = new Player(GameStateService.SavedPlayerInfo.Username);

            WebSocketHandler.OnReadyMessages.Add(new ReconnectMessage() { Username = GameStateService.SavedPlayerInfo.Username});
        }
        public static Sides GetSide(bool value)
        {
            return value ? Sides.Cross : Sides.Nought;
        }

        public static async Task JoinPlayer(string username, AvailableGame game)
        {
            Connection = GameConnection.Connect;

            EnemyPlayer = new Player(game.Username) { Id = game.PlayerId };
            CurrentPlayer = new Player(username);
            WebSocketHandler.OnReadyMessages.Add(new InitialJoinMessage() { RoomName = game.RoomName, Username = username });
            CurrentGamePage = new TicTacToePage(PageCreated, game.Size);
            SizeMap = game.Size;
            _ = MainThread.InvokeOnMainThreadAsync(()=>{
                CurrentGamePage.ServerState.Text = $"Connection to {EnemyPlayer.Username}...";
                });
            await GameStateService.Navigation.PushAsync(CurrentGamePage);
        }
        public static async Task CreateRoom(string username, string room, int size)
        {
            Connection = GameConnection.Create;
            CurrentPlayer = new Player(username);
            Debug.WriteLine($"username: {username}, room: {room}");
            WebSocketHandler.OnReadyMessages.Add(new InitialCreateMessage() { RoomName = room, Username = username, Size = size });
            SizeMap = size;
            CurrentGamePage = new TicTacToePage(PageCreated, size);
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
                CurrentGamePage.OnGameCancel += CurrentGamePage_OnGameCancel;
            }
        }

        private static async void CurrentGamePage_OnGameCancel()
        {
            await WebSocketHandler.Close();
            Debug.WriteLine("lol");
            await GameStateService.Navigation.PopAsync();
            Debug.WriteLine("lol1");
            GameStateService.SavedPlayerInfo = CurrentPlayer;
            Debug.WriteLine("lol2");
            GameStateService.Reset();
            Debug.WriteLine("lol3");
            Connection = null;
            Debug.WriteLine("lol4");
            IsGameRunning = false;
            Debug.WriteLine("lol5");
            CurrentGamePage = null;
            Debug.WriteLine("lol6");
            EnemyPlayer = null;
            Debug.WriteLine("lol7");
            CurrentPlayer = null;
            Debug.WriteLine("lol8");
            _currentSideMove = FirstSideMove;
            Debug.WriteLine("lol9");
        }

        private static void CurrentGamePage_OnGameAreaCreate()
        {
            
        }
        private static bool IsThereFreeCellInArea()
        {
            foreach(List<CellButton> row in CurrentGamePage.CellList)
            {
                foreach(CellButton cell in row)
                {
                    if (!cell.Closed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool IsItWinMove(CellButton cell)
        {
            int count = 0;
            int count2 = 0;
            for(int row = 0; row < CurrentGamePage.DefaultCellsInRow; row++)
            {
                for(int col = 0; col < CurrentGamePage.DefaultCellsInColumn; col++)
                {
                    if (CurrentGamePage.CellList[row][col].Side == CurrentPlayer.Side)
                    {
                        count++;
                    }
                    if (CurrentGamePage.CellList[col][row].Side == CurrentPlayer.Side)
                    {
                        count2++;
                    }
                }
                if(count == CurrentGamePage.DefaultCellsToWin || count2 == CurrentGamePage.DefaultCellsToWin)
                {

                    return true;
                }
                else 
                {
                    count = 0;
                    count2 = 0;
                }
            }
            count = 0;
            count2 = 0;

            int column = 0;
            for(int row = 0; row < CurrentGamePage.DefaultCellsInRow; row++)
            {
                if (CurrentGamePage.CellList[row][column].Side == CurrentPlayer.Side)
                {
                    count++;
                }
                if (CurrentGamePage.CellList[row][CurrentGamePage.DefaultCellsInColumn - 1 - column].Side == CurrentPlayer.Side)
                {
                    count2++;
                }
                column++;
            }
            if(count == CurrentGamePage.DefaultCellsToWin || count2 == CurrentGamePage.DefaultCellsToWin)
            {
                return true;
            }
            return false;
        }

        private static async void CurrentGamePage_OnCellClick(CellButton cell)
        {
            if(!IsGameRunning) return;
            if(WebSocketHandler.GetWebSocketState() == System.Net.WebSockets.WebSocketState.Open)
            {
                if(CurrentPlayer != null && CurrentSideMove == CurrentPlayer.Side && !cell.Closed && cell.CellInGameArea != null && CurrentPlayer.Side != null)
                {
                    
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CurrentGamePage.ColourCell((Sides)CurrentPlayer.Side, false, cell);
                    });
                    cell.Closed = true;
                    cell.Side = CurrentPlayer.Side;
                    CurrentSideMove = (Sides)EnemyPlayer.Side;
                    await WebSocketHandler.SendMessage(new PlayerMove() { X = cell.CellInGameArea.X , Y = cell.CellInGameArea.Y });
                    if (IsItWinMove(cell))
                    {
                        IsGameRunning = false;
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            CurrentGamePage.ServerState.Text = "You won!";
                        });
                        await WebSocketHandler.SendMessage(new PlayerWon());
                    }
                    else if (!IsThereFreeCellInArea())
                    {
                        IsGameRunning = false;
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            CurrentGamePage.ServerState.Text = "Draw!";
                        });
                        await WebSocketHandler.SendMessage(new Draw());
                    }
                }
            }
        }
    }
}
