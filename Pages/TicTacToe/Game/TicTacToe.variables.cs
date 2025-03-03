using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alexm_app.Controls;
using alexm_app.Enums.TicTacToe;
using alexm_app.Models;
using Game = alexm_app.Models.TicTacToe.Game;

namespace alexm_app
{
    public partial class TicTacToePage
    {
        public Grid GameArea { get; set; } = new Grid()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        public VerticalStackLayout MainContainer { get; set; } = new VerticalStackLayout();
        public List<List<CellButton>> CellList { get; set; } = new List<List<CellButton>>();
        public Button CancelGameButton { get; set; } = new Button() { Text = "Cancel" } ;
        public Label ServerState { get; set; } = new Label();
        public Label CurrentSide { get; set; } = new Label();
        public Label PlayerSide { get; set; } =  new Label();
        public Picker ThemePicker { get; set; } 
        public int DefaultCellsInRow { get; set; } = 2;
        public int DefaultCellsInColumn { get; set; } = 2;
        public int DefaultCellsToWin { get; set; } = 2;
        public Sides? CurrentEnemySide { get; set; } = null;
        public Sides? CurrentPlayerSide { get; set; } = null;
        
    }
}
