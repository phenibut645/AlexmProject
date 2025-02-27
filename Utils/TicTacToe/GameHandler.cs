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
using alexm_app;
using alexm_app.Services;
using alexm_app.Controls;
namespace alexm_app.Utils.TicTacToe
{
    public static class GameHandler
    {
        static GameHandler()
        {
            
        }
        public static void GeneratePlayer(string username, Sides? side = null)
        {
            Debug.WriteLine("Generating player in context...");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string stringChars = "";

            for (int i = 0; i < 10; i++)
            {
                stringChars += chars[random.Next(chars.Length)];
            }
           
            AppContext.UniqueIdentity = stringChars.ToString();
            AppContext.Player = new Player(username);
            if(side != null) AppContext.Player.Side = side;
        }
        public static void GenerateGame(AvailableGame game)
        {
            Debug.WriteLine("Generating game in context...");
            AppContext.Game = new Game(AppContext.TicTacToeTheme, GameMode.Multiplayer, game.RoomName, currentTurn: Sides.Cross);
        }
        public static void GenerateGame(string roomName)
        {
            Debug.WriteLine("Generating game in context...");
            AppContext.Game = new Game(AppContext.TicTacToeTheme, GameMode.Multiplayer, roomName);
        }
        public static async Task JoinPlayer(string username, AvailableGame game)
        {
            GeneratePlayer(username);
            GenerateGame(game);
            SServerHandler.OnConnectionComplete += SServerHandler_OnConnectionComplete;
            await DatabaseHandler.CreatePlayer(game.RoomName);
            Debug.WriteLine($"UniqueIdentity {AppContext.UniqueIdentity}, Username {AppContext.Player.Username}");
            EnemyPlayer? enemy = null;
            while(enemy == null)
            {
                enemy = await DatabaseHandler.GetEnemyPlayer();
            }
            if(enemy != null)
            {
                Debug.WriteLine("\nenemy is not null\n");
                AppContext.EnemyPlayer = new Player(enemy.EnemyPlayerUsername) { Id = enemy.EnemyPlayerId };
            }
            else
            {
                Debug.WriteLine("enemy not found in database");
            }
            SServerHandler.OnReadyMessages.Add(new InitialJoinMessage() { RoomName = game.RoomName, UniqueIdentity = AppContext.UniqueIdentity, Username = AppContext.Player.Username });
            
            Debug.WriteLine($"{username} wanted to join {game.RoomName} room");
            AppContext.GameConnection = GameConnection.Connect;
            AppContext.GameMode = GameMode.Multiplayer;
            await AppContext.Navigation.PushAsync(new alexm_app.TicTacToe());
        }

        private static void SServerHandler_OnConnectionComplete(Models.TicTacToe.ServerMessages.WebSocket.ConnectionCompleted message)
        {
            AppContext.Player!.Side = message.Turn ? Sides.Cross : Sides.Nought;
            AppContext.CurrentGame.PlayerSide.Text = $"Your side is: {AppContext.Player.Side}";
            AppContext.EnemyPlayer!.Side = message.Turn ? Sides.Nought : Sides.Cross;
        }

        public static async void CreateGame(string roomName, string username)
        {
            GeneratePlayer(username);
            GenerateGame(roomName);

            await DatabaseHandler.CreateGame();
            await DatabaseHandler.CreatePlayer();
            Debug.WriteLine("Adding initial messages...");
           
            SServerHandler.OnReadyMessages.Add(new InitialCreateMessage() { RoomName = roomName, UniqueIdentity = AppContext.UniqueIdentity, Username = username });
            SServerHandler.OnPlayerConnect += SServerHandler_OnPlayerConnect;
            AppContext.GameConnection = GameConnection.Create;
            AppContext.GameMode = GameMode.Multiplayer;
            await AppContext.Navigation.PushAsync(new alexm_app.TicTacToe());
        }

        private static void SServerHandler_OnPlayerConnect(Models.TicTacToe.ServerMessages.WebSocket.PlayerConnected message)
        {
           AppContext.Player!.Side = message.Turn ? Sides.Cross : Sides.Nought;
            AppContext.CurrentGame.PlayerSide.Text = $"Your side is: {AppContext.Player.Side}";
           Task.Run(async () =>
            {
                EnemyPlayer enemy = await DatabaseHandler.GetEnemyPlayer();
                AppContext.EnemyPlayer = new Player(enemy.EnemyPlayerUsername) { Id = enemy.EnemyPlayerId , Side = message.Turn ? Sides.Nought : Sides.Cross };
            });
        }

        public static async void Reconnect()
        {
            await SServerHandler.SendMessage(new ReconnectMessage());
        }
        public static Button GetReportButton(ContentPage page)
        {
            Button button = new Button() { Text = "Send report" };
            button.Clicked += async(object? sender, EventArgs args) =>
            {
                string response = await page.DisplayPromptAsync("Send report", "What's is your issue?");
                await ReportSender.SendMessage(response);
            };
            return button;
        }
        public static bool MultiplayerValidation(bool sideValidation = false)
        {
            List<object?> objectForValidation = new List<object?>()
            {
                AppContext.Game, AppContext.UniqueIdentity, AppContext.GameMode, AppContext.CurrentGame, AppContext.EnemyPlayer, AppContext.GameConnection
            };
            if(sideValidation) objectForValidation.Add(AppContext.Side);
            foreach(object? obj in objectForValidation){
                if(obj == null) return false;
            }
            return true;
        }
        public static Sides GetSide(bool var)
        {
            if(var) return Sides.Nought;
            else return Sides.Cross;
        }
        public static void ResetConfiguration()
        {

            AppContext.Game = null;
            AppContext.UniqueIdentity = null;
            AppContext.GameMode = null;
            AppContext.CurrentGame = null;
            AppContext.EnemyPlayer = null;
            AppContext.GameConnection = null;
            AppContext.IsGameRunning = null;
        }
        public static async Task EndGame()
        {
            Debug.WriteLine("MF 1 " + AppContext.Game?.Id);
            await DatabaseHandler.ClearGame();
            Debug.WriteLine("2");
            ResetConfiguration();
            Debug.WriteLine("3");
            await SServerHandler.Close();
        }
        public static void GamePageCreated()
        {
            AppContext.CurrentGame.OnCellClick += CurrentGame_OnCellClick;
            SServerHandler.OnPlayerMove += SServerHandler_OnPlayerMove;
            SServerHandler.OnPlayerWin += SServerHandler_OnPlayerWin;
        }

        private static async void SServerHandler_OnPlayerWin()
        {
            AppContext.CurrentGame.CurrentSide.Text = $"{AppContext.EnemyPlayer.Username} won!";
            await SServerHandler.Close();
            ResetConfiguration();
        }

        private static void SServerHandler_OnPlayerMove(Models.TicTacToe.ServerMessages.WebSocket.PlayerMoved message)
        {
            AppContext.Game.CurrentTurn = AppContext.Player.Side;
            AppContext.Side = AppContext.Player.Side;
            
        }

        private static async void CurrentGame_OnCellClick(Models.TicTacToe.Cell cell)
        {
            if(AppContext.Game!.CurrentTurn == AppContext.Player!.Side)
            {
                Controls.CellButton cellControl = AppContext.CurrentGame.CellList[cell.Y][cell.X];

                cellControl.Text = GetSymbol((Enums.TicTacToe.Sides)AppContext.Player.Side);
                cellControl.BackgroundColor = Color.FromRgba("#7dff99");
                cellControl.Closed = true;
                cellControl.Side = AppContext.Player.Side;
                await DatabaseHandler.Move(cell);
                if (CheckWin(cell))
                {
                    AppContext.CurrentGame.CurrentSide.Text = "Yow won!";
                    Debug.WriteLine("started ------------------------------------------------------------");
                    await SServerHandler.SendMessage(new PlayerWon());
                    await EndGame();
                }
                else
                {
                    AppContext.Side = AppContext.EnemyPlayer.Side;
                    AppContext.Game.CurrentTurn = AppContext.EnemyPlayer.Side;

                    
                    await DatabaseHandler.ChangeTurn();
                    
                }
                await SServerHandler.SendMessage(new PlayerMove() { X = cell.X, Y = cell.Y });
                
            }
            else await AppContext.CurrentGame!.DisplayAlert("Message", "Now it's not your turn!", "OK");
        }
        private static bool CheckWin(Models.TicTacToe.Cell cell)
        {
            List<List<CellButton>> cells = AppContext.CurrentGame.CellList;
            int count = 0;
            foreach(List<CellButton> row in cells)
            { 
                foreach(CellButton column in row)
                {
                    if(column.Side == AppContext.Player.Side) count += 1;
                    else
                    {
                        count = 0;
                        break;
                    }
                }
                if(count == 3) return true;
            }
            count = 0;
            for(int column = 0; column < 3 ; column++)
            {
                for(int row = 0; row < 3; row++)
                {
                    if (cells[row][column].Side == AppContext.Player.Side) count++;
                    else
                    {
                        count = 0;
                        break;
                    }
                }
                if(count == 3) return true;
            }
            return false;
        }
        public static string GetSymbol(Sides side)
        {
            if(side == Sides.Cross) return "X";
            else return "0";
        }
    }
}
