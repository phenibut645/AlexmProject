using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ClientMessages.WebSocket
{
    public class InitialJoinMessage: ClientMessage
    {
        [JsonProperty("message_type")]
        public string MessageType { get; set; } = "InitialJoinMessage";
        [JsonProperty("unique_identity")]
        public string UniqueIdentity { get; set; }
        [JsonProperty("room_name")]
        public string RoomName { get; set; }
        [JsonProperty("username")]
        public string Username { get; set;}
    }
}
