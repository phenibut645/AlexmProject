using alexm_app.Controls;
using alexm_app.Enums.TicTacToe;
using alexm_app.Utils.TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app
{
    public partial class TicTacToePage
    {
        public void StartGame()
        {

        }
        public CellButton GetCellButton(int x, int y)
        {
            return CellList[y][x];
        }
        public void ColourCell(Sides side, bool enemy, CellButton cell)
        {
            cell.BackgroundColor = enemy ? GameStateService.TicTacToeTheme.EnemyCell : GameStateService.TicTacToeTheme.PlayerCell;
            cell.Text = side == Sides.Cross ? "X" : "O";
        }
        public void ColourCell(Sides side, bool enemy, int x, int y)
        {
            CellButton cell = GetCellButton(x, y);
            cell.BackgroundColor = enemy ? GameStateService.TicTacToeTheme.EnemyCell : GameStateService.TicTacToeTheme.PlayerCell;
            cell.Text = side == Sides.Cross ? "X" : "O";
        }
    }
}
