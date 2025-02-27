using alexm_app.Enums.TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe
{
    public class Player
    {
        public string Username { get; set;}
        public Sides? Side { get; set; }
        public int Id { get; set; }
        public Player(string username)
        {
            Username = username;
        }
    }
}
