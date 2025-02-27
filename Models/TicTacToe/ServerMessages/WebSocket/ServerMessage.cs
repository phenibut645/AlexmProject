using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ServerMessages.WebSocket
{
    public interface ServerMessage
    {
        [JsonProperty("message_type")]
        public string MessageType { get; set; }
    }
}
