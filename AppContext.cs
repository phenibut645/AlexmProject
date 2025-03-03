using alexm_app.Enums.TicTacToe;
using alexm_app.Models;
using alexm_app.Models.TicTacToe;
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
            new GuestPage()
        };
        public static ContentPage CurrentPage;
    }
}
