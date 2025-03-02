using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ServerMessages
{
    public class CreateUserByRoomName
    {
        [JsonProperty("player_id")]
        public int PlayerId { get; set; }
        [JsonProperty("game_id")]
        public int GameId { get; set; }
    }
}
