using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ClientMessages
{
    public class ConnectPlayerMessage: IClientMessage
    {
        public string Request { get; set; } = "ConnectPlayer";
        public string Player { get; set; }
        public string RoomName { get; set; }
        public string UniqueIdentity { get; set; }
    }
}
