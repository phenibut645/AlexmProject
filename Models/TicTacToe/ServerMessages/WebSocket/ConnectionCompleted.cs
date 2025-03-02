using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ServerMessages.WebSocket
{
    public class ConnectionCompleted: ServerMessage
    {
        [JsonProperty("message_type")]
        public string MessageType { get; set;}
        [JsonProperty("unique_identity")]
        public string UniqueIdentity { get; set; }

        [JsonProperty("turn")]
        public bool Turn { get; set; }
    }
}
