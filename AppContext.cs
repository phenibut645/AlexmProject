using alexm_app.Enums.TicTacToe;
using alexm_app.Models;
using alexm_app.Models.TicTacToe;
using alexm_app.Pages.Countries;
using alexm_app.Pages.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app
{

    public static class AppContext
    {
        
        public static List<ContentPage> Pages { get; set; } = new List<ContentPage>()
        {
            new ValgusfoorPage(),
            new rgb_page(),
            new lumimamm(),
            new GuestPage(),
            new FriendsPage(),
            new CountriesPage()

        };
        public static ContentPage CurrentPage;
    }
}
