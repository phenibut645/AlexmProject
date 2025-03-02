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
    }
}
