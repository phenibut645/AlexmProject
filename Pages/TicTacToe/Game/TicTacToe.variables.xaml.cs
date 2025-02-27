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
    public partial class TicTacToe
    {
        public Grid GameArea { get; set; } = new Grid()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        public VerticalStackLayout MainContainer { get; set; } = new VerticalStackLayout();
        public List<List<CellButton>> CellList { get; set; } = new List<List<CellButton>>();
        public Button CancelGameButton { get; set; } = new Button() { Text = "Cancel" } ;
        public Label ServerState { get; set; } = new Label() { Text = "Waiting for player", TextColor = Color.FromArgb("#ffffff")};
        public Label CurrentSide { get; set; } = new Label() { Text = $"Current side: {AppContext.Side}", TextColor = Color.FromArgb("#ffffff")};
        public Label PlayerSide { get; set; } =  new Label() { Text = $"Your side: {AppContext.Player.Side.ToString()}", TextColor = Color.FromArgb("#ffffff")};
    }
}
