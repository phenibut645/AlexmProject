using alexm_app.Models.TicTacToe;
using alexm_app.Models.TicTacToe.ServerMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
        public static async Task<bool> IsUsernameAvailable(string username)
        {
            using(HttpClient client = new HttpClient())
            {
                try
                {
                    UriBuilder uriBuilder = new UriBuilder(API_URL + "is_there_player.php");
                    NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                    query["username"] = username;
                    uriBuilder.Query = query.ToString();
                    HttpResponseMessage response = await client.GetAsync(uriBuilder.ToString());
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Dictionary<string, bool>? desirilizedObject = JsonConvert.DeserializeObject<Dictionary<string, bool>>(responseBody);
                    Debug.WriteLine($"username available response: {responseBody}");
                    bool value;
                    if(desirilizedObject != null && desirilizedObject.TryGetValue("player", out value) == true)
                    {
                        return !value;
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine("IsUsernameAvailable " + e.ToString());
                }
            }
            return false;
        }
        public static async Task<bool> IsRoomNameAvailable(string roomname)
        {
            using(HttpClient client = new HttpClient())
            {
                try
                {
                    UriBuilder uriBuilder = new UriBuilder(API_URL + "is_there_game.php");
                    NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                    query["room"] = roomname;
                    uriBuilder.Query = query.ToString();
                    HttpResponseMessage response = await client.GetAsync(uriBuilder.ToString());
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Dictionary<string, bool>? deserializedObject = JsonConvert.DeserializeObject<Dictionary<string, bool>>(responseBody);
                    Debug.WriteLine($"game available response: {responseBody}");
                    if(deserializedObject != null)
                    {
                        bool value;
                        if(deserializedObject.TryGetValue("game", out value) == true)
                        {
                            return !value;
                        }
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine($"IsRoomnameAvailable {e}");
                }
               
            }
            return false;
        }
    }

}
