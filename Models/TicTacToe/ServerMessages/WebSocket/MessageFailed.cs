using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ServerMessages.WebSocket
{
    public class MessageFailed: ServerMessage
    {
        [JsonProperty("message_type")]
        public string MessageType { get; set; }

        [JsonProperty("message")]
        public string Message { get; set;}
    }
}
