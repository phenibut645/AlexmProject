using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ServerMessages
{
    public class EnemyPlayer
    {
        [JsonProperty("id")]
        public int EnemyPlayerId { get; set; }
        [JsonProperty("username")]
        public string EnemyPlayerUsername { get; set; }
    }
}
