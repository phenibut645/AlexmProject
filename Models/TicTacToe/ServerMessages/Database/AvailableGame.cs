using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ServerMessages
{
    public class AvailableGame
    {
        [JsonProperty("game_id")]
        public int GameId { get; set; }

        [JsonProperty("room_name")]
        public string RoomName { get; set; }

        [JsonProperty("player_id")]
        public int PlayerId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }
}
