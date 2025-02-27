using alexm_app.Enums.TicTacToe;
using Cell = alexm_app.Models.TicTacToe.Cell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Controls
{
    public partial class CellButton: Button
    {
        public Sides? Side { get; set; }
        public Cell? CellInGameArea { get; set; }
        public bool Closed { get; set; } = false;
    }
}
