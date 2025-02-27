using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alexm_app.Enums.TicTacToe;
using alexm_app.Utils.TicTacToe;

namespace alexm_app.Models.TicTacToe
{
    public class Game
    {
        public int? Id { get; set; } = null;
        public string? RoomName { get; set; }
        public bool Finished { get; set; } = false;
        public Sides? CurrentTurn { get; set; } = Sides.Cross;
        public GameMode GameMode { get; set; }
        public Moves Moves { get; set; } = new Moves();
        public DateTime Date { get; set; } = DateTime.Now;
        public Theme Theme { get; set; }
        public Game(Theme theme, GameMode gameMode, string roomName, Sides? currentTurn = null)
        {
            Theme = theme;
            GameMode = gameMode;
            if (currentTurn != null) CurrentTurn = currentTurn;
            RoomName = roomName;
        }
    }
}
