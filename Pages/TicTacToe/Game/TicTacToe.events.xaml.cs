using alexm_app.Controls;
using alexm_app.Models.TicTacToe;
using alexm_app.Models.TicTacToe.ServerMessages.WebSocket;
using alexm_app.Utils.TicTacToe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app
{
    public delegate void CellClickedDelegate(Models.TicTacToe.Cell cell);
    public partial class TicTacToe
    {
        public event CellClickedDelegate OnCellClick;
        private void AddEventListeners()
	    {
            AppContext.TicTacToeTheme.onBackgroundColorChange += Theme_onBackgroundColorChange;
            AppContext.TicTacToeTheme.onCellColorChange += Theme_onCellColorChange;
            AppContext.TicTacToeTheme.onTextColorChanged += Theme_onTextColorChanged;
            AppContext.TicTacToeTheme.onButtonColorChange += Theme_onButtonColorChange;
            AppContext.TicTacToeTheme.onFrameColorChange += Theme_onFrameColorChange;
            CancelGameButton.Clicked += CancelGameButton_Clicked;
            if(AppContext.GameMode == Enums.TicTacToe.GameMode.Multiplayer)
            {
                SServerHandler.OnWebSocketClose += SServerHandler_OnWebSocketClose;
                SServerHandler.OnPlayerMove += SServerHandler_OnPlayerMove;
                if(AppContext.GameConnection == Enums.TicTacToe.GameConnection.Create)
                {
                    SServerHandler.OnPlayerConnect += SServerHandler_OnPlayerConnect;
                }
                else
                {
                    SServerHandler.OnConnectionComplete += SServerHandler_OnConnectionComplete;
                }
                
            }
        }
        private void AppContext_OnSideChange()
        {
            CurrentSide.Text = $"Current side: {AppContext.Side.ToString()}";
        }
        private void SServerHandler_OnPlayerMove(PlayerMoved message)
        {
            Debug.WriteLine("PLAYER MOVED -----------------------------------------------------");
            CellList[message.Y][message.X].BackgroundColor = Color.FromRgba("#ff4f75");
            if (AppContext.EnemyPlayer?.Side != null)
            {
                CellList[message.Y][message.X].Text = GameHandler.GetSymbol((Enums.TicTacToe.Sides)AppContext.EnemyPlayer.Side);
                CellList[message.Y][message.X].Side = AppContext.EnemyPlayer.Side;
            }
            else
            {
                Debug.WriteLine("enemy player side is null -----------------------------------------------------");
            }
             CellList[message.Y][message.X].Closed = true;
        }

        private async void SServerHandler_OnWebSocketClose()
        {
            if (AppContext.IsGameRunning == true) await DisplayAlert("WebSocket connection", "WebSocket connection is closed!", "OK");
            GameHandler.EndGame();
        }

        private void SServerHandler_OnConnectionComplete(ConnectionCompleted message)
        {
            
            if (SServerHandler.GetWebSocketState() == System.Net.WebSockets.WebSocketState.Open)
            {
                ServerState.Text = $"Connection completed! Your enemy is {AppContext.EnemyPlayer!.Username}";
            }
        }

        private void SServerHandler_OnPlayerConnect(Models.TicTacToe.ServerMessages.WebSocket.PlayerConnected message)
        {
            Debug.WriteLine("component event onplayerconnect");
            
            ServerState.Text = $"{message.PlayerUsername} has connected!";
            Task.Run(async () => {
                Task.Delay(5000);
                ServerState.Text = $"{message.PlayerUsername}";
                });
        }

        private async void CancelGameButton_Clicked(object? sender, EventArgs e)
        {
            await SServerHandler.Close();
        }

        private void Theme_onFrameColorChange(Color color)
        {

        }

        private void Theme_onButtonColorChange(Color color)
        {
            
        }

        private void Theme_onTextColorChanged(Color color)
        {
            foreach(List<CellButton> cellList in CellList) {
                foreach(CellButton cell in cellList)
                {
                    cell.TextColor = color;
                }
            }
        }

        private void Theme_onCellColorChange(Color color)
        {
            foreach(List<CellButton> cellList in CellList) {
                foreach(CellButton cell in cellList)
                {
                    cell.BackgroundColor = color;
                }
            }
        }

        private void Theme_onBackgroundColorChange(Color color)
        {
		    MainContainer.BackgroundColor = color;
        }
        private void CellButton_Clicked(object? sender, EventArgs e)
        {
            CellButton? cellButton = sender as CellButton;
            if(cellButton != null && !cellButton.Closed)
            {
                
                OnCellClick?.Invoke(cellButton.CellInGameArea);
            }
        }
    }
}
