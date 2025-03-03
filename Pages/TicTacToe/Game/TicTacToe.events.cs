using alexm_app.Controls;
using alexm_app.Models.TicTacToe;
using alexm_app.Models.TicTacToe.ServerMessages.WebSocket;
using alexm_app.Services;
using alexm_app.Utils.TicTacToe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            theme.CallEveryEvent();
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
