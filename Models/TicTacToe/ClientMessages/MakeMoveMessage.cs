using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ClientMessages
{
    public class MakeMoveMessage: IClientMessage
    {
        public string Request { get; set; } = "MakeMove";
        public int X { get; set;}
        public int Y { get; set;}
        public string RoomName { get; set;}
        public string Player { get; set; }
    }
}
