using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ClientMessages.WebSocket
{
    public class InitialCreateMessage: ClientMessage
    {
        [JsonProperty("message_type")]
        public string MessageType { get; set; } = "InitialCreateMessage";
        [JsonProperty("room_name")]
        public string RoomName { get; set;}
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }
}
