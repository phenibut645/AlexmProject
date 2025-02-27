using alexm_app.Models.TicTacToe;
using alexm_app.Models.TicTacToe.ServerMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Utils.TicTacToe
{
    public static class DatabaseHandler
    {
        public static string API_URL { get; private set; } = @"https://aleksandermilisenko23.thkit.ee/other/tic-tac-toe/api/";
        public static async Task<List<AvailableGame>?> GetAvailableGames()
        {
            using(HttpClient client = new HttpClient())
            {
                string url = API_URL+ "available_games.php";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string stringResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(stringResponse);
                    List<AvailableGame>? responseData = JsonConvert.DeserializeObject<List<AvailableGame>>(stringResponse);
                    if(responseData != null)
                    {
                        Debug.WriteLine(await response.Content.ReadAsStringAsync());
                        Debug.WriteLine("\n===========================\nnot null\n========================\n");
                        foreach(AvailableGame item in responseData)
                        {
                            Debug.WriteLine($"{item.RoomName}");
                        }
                        return responseData;
                    }
                }
                return null;
            }
        }
        public static async Task CreateGame()
        {
            if(AppContext.Game == null) return;
            using(HttpClient client = new HttpClient())
            {
                string url = API_URL + "create_game.php";
                var content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "room", AppContext.Game.RoomName} });
                try
                {
                    Debug.WriteLine("Creating game in the database...");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Game has created in the database.");
                        CreateGame? responseBody = JsonConvert.DeserializeObject<CreateGame>(await response.Content.ReadAsStringAsync());
                        if(responseBody != null) {
                            Debug.WriteLine("Game id: " + responseBody.GameId);
                            AppContext.Game.Id = responseBody.GameId;
                            Debug.WriteLine("NOW IT'S : " + AppContext.Game.Id + " ===========================");
                        }
                        Debug.WriteLine(responseBody);
                    }
                }
                catch
                {

                }
            }
        }
        public static async Task CreatePlayer(string? room_name = null)
        {
            if(AppContext.Player == null || AppContext.Game == null) return;
            using(HttpClient client = new HttpClient())
            {
                string url = API_URL + "create_user.php";
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    { "username", AppContext.Player.Username },
                    { "unique_identity", AppContext.UniqueIdentity }
                };
                if(AppContext.Game.Id == null) dict.Add("room_name", room_name);
                else dict.Add("game", AppContext.Game.Id.ToString());
                var content = new FormUrlEncodedContent(dict);
                try
                {
                    Debug.WriteLine("Creating player in the database...");
                    foreach(KeyValuePair<string, string> items in dict)
                    {
                        Debug.WriteLine($"{items.Key} => {items.Value}");
                    }
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Player has created." + await response.Content.ReadAsStringAsync());
                        if(AppContext.Game.Id != null)
                        {
                            CreateUser? responseBody = JsonConvert.DeserializeObject<CreateUser>(await response.Content.ReadAsStringAsync());
                            if(responseBody!= null)
                            {
                                AppContext.Player.Id = responseBody.PlayerId;
                            }
                        }
                        else
                        {
                            CreateUserByRoomName? responseBody = JsonConvert.DeserializeObject<CreateUserByRoomName>(await response.Content.ReadAsStringAsync());
                            if(responseBody!= null)
                            {
                                Debug.WriteLine("Game id have found for us " + responseBody.GameId);
                                AppContext.Player.Id = responseBody.PlayerId;
                                AppContext.Game.Id = responseBody.GameId;
                            }
                        }
                        
                    }
                }
                catch
                {

                }
            }
        }
        public static async Task ChangeTurn()
        {
            if(AppContext.IsGameRunning == null || AppContext.IsGameRunning == false || AppContext.Game == null || AppContext.Player == null || AppContext.Game.Id == null || AppContext.Player.Id == null) return;
            using(HttpClient client = new HttpClient())
            {
                string url = API_URL + "change_turn.php";
                Dictionary<string, string> options = new Dictionary<string, string>()
                {
                    {"game", AppContext.Game.Id.ToString() },
                    {"player", AppContext.Player.Id.ToString() }
                };
                var content = new FormUrlEncodedContent(options);
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);
                }
                catch
                {

                }
            }
        }
        public static async Task<EnemyPlayer?> GetEnemyPlayer()
        {
            using(HttpClient client = new HttpClient())
            {
                Debug.WriteLine("trying to find enemy player");
                string url = API_URL + $"enemy_player.php?player={AppContext.Player.Id}";
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    string json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("enemy " + json);
                    EnemyPlayer? enemyPlayer = JsonConvert.DeserializeObject<EnemyPlayer>(json);
                    if (enemyPlayer != null) return enemyPlayer;

                }
                catch
                {
                    Debug.WriteLine("Error");
                }
            }
            return null;
        }
        public static async Task Move(Models.TicTacToe.Cell cell)
        {
            using(HttpClient client = new HttpClient())
            {
                string url = API_URL + "move.php";
                Dictionary<string, string> properties = new Dictionary<string, string>()
                {
                    { "x", cell.X.ToString() },
                    { "y", cell.Y.ToString() },
                    { "game", AppContext.Game.Id.ToString() },
                    { "player", AppContext.Player.Id.ToString() }
                };
                Debug.WriteLine(AppContext.Game.Id.ToString() + " move, our game id ----------============---------========--------");
                var content = new FormUrlEncodedContent(properties);
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);
                }
                catch
                {
                    Debug.WriteLine("move end point has crashed");
                }
            }
        }
        public static async Task ClearGame()
        {
            if(AppContext.Game == null) return;

            using(HttpClient client = new HttpClient())
            {
                string url = API_URL + "clear_game.php";
                string id = AppContext.Game.Id.ToString();
                Debug.WriteLine("1.1 " + AppContext.Game?.Id);
                Dictionary<string, string> properties = new Dictionary<string, string>()
                {
                    { "game", id }
                };

                Debug.WriteLine("1.2 " + id);
                var content = new FormUrlEncodedContent(properties);
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    Debug.WriteLine("1.3 " + AppContext.Game?.Id);
                }
                catch
                {
                    Debug.WriteLine("clear_game end point have crashed");
                }
            }
        }
    }
}
