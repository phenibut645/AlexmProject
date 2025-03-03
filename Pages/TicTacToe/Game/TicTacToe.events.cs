using alexm_app.Controls;
using alexm_app.Models.TicTacToe;
using alexm_app.Models.TicTacToe.ServerMessages.WebSocket;
using alexm_app.Services;
using alexm_app.Utils.TicTacToe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app
{
    public delegate void CellClickedDelegate(CellButton cell);
    public delegate void GameAreaCreatedDelegate();
   
    public partial class TicTacToePage
    {
        public event CellClickedDelegate OnCellClick;
        public event GameAreaCreatedDelegate OnGameAreaCreate;
        public event Action OnGameCancel;
        private void AddEventListeners()
	    {
            GameStateService.OnThemeChange += GameStateService_OnThemeChange;
            GameStateService.TicTacToeTheme.onBackgroundColorChange += Theme_onBackgroundColorChange;
            GameStateService.TicTacToeTheme.onCellColorChange += Theme_onCellColorChange;
            GameStateService.TicTacToeTheme.onTextColorChanged += Theme_onTextColorChanged;
            GameStateService.TicTacToeTheme.onButtonColorChange += Theme_onButtonColorChange;
            GameStateService.TicTacToeTheme.onFrameColorChange += Theme_onFrameColorChange;

            CancelGameButton.Clicked += CancelGameButton_Clicked1;
        }

        private void GameStateService_OnThemeChange(Models.Theme theme)
        {
            GameStateService.TicTacToeTheme.onBackgroundColorChange += Theme_onBackgroundColorChange;
            GameStateService.TicTacToeTheme.onCellColorChange += Theme_onCellColorChange;
            GameStateService.TicTacToeTheme.onTextColorChanged += Theme_onTextColorChanged;
            GameStateService.TicTacToeTheme.onButtonColorChange += Theme_onButtonColorChange;
            GameStateService.TicTacToeTheme.onFrameColorChange += Theme_onFrameColorChange;
            GameStateService.TicTacToeTheme.OnPlayerCellColorChange += TicTacToeTheme_OnPlayerCellColorChange;
            GameStateService.TicTacToeTheme.OnEnemyCellColorChange += TicTacToeTheme_OnEnemyCellColorChange;
            theme.CallEveryEvent();
        }

        private void TicTacToeTheme_OnEnemyCellColorChange(Color color)
        {
            foreach(List<CellButton> row in CellList)
            {
                foreach(CellButton cell in row)
                {
                    if(cell.Side != null && CurrentEnemySide != null && cell.Side == CurrentEnemySide)
                    {
                        cell.BackgroundColor = color;
                    }
                }
            }
        }

        private void TicTacToeTheme_OnPlayerCellColorChange(Color color)
        {
            foreach(List<CellButton> row in CellList)
            {
                foreach(CellButton cell in row)
                {
                    if(cell.Side != null && CurrentPlayerSide != null && cell.Side == CurrentPlayerSide)
                    {
                        cell.BackgroundColor = color;
                    }
                }
            }
        }

        private void CancelGameButton_Clicked1(object? sender, EventArgs e)
        {
            OnGameCancel?.Invoke();
        }

        private void Theme_onFrameColorChange(Color color)
        {
            GameArea.BackgroundColor = color;
        }

        private void Theme_onButtonColorChange(Color color)
        {
            CancelGameButton.BackgroundColor = color;
        }

        private void Theme_onTextColorChanged(Color color)
        {
            foreach(List<CellButton> cellList in CellList) {
                foreach(CellButton cell in cellList)
                {
                    cell.TextColor = color;
                }
            }
            ServerState.TextColor = color;
            CurrentSide.TextColor = color;
            PlayerSide.TextColor = color;
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
                OnCellClick?.Invoke(cellButton);
            }
        }
    }
}
