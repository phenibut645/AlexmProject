using alexm_app.Models;
using alexm_app.Utils.TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Services
{
    public static class GameService
    {
        public static List<Theme> Themes = new List<Theme>()
	    {
		    new Theme(),
		    new Theme(Color.FromArgb("#c9ffe6"), Color.FromArgb("#b0faff"), Color.FromArgb("#f0f0f0"), Color.FromArgb("#8c73bd"), Color.FromArgb("#adff9e")) { Name = "Banana theme" },
            new Theme(Color.FromArgb("#210000"), Color.FromArgb("#423333"), Color.FromArgb("#ffffff"), Color.FromArgb("#20004a"), Color.FromArgb("#d10046")) {Name = "Bloody"}
	    };
        public static Picker GetThemePicker()
        {
            Picker ThemePicker = new Picker() { Title = "Select a theme"};
            List<string> themesName = new List<string>();
		    foreach(Theme theme in Themes)
		    {
			    themesName.Add(theme.Name);
		    }
		    ThemePicker.ItemsSource = themesName;
		    ThemePicker.SelectedIndex = 0;
            ThemePicker.SelectedIndexChanged += ThemePicker_SelectedIndexChanged;

            return ThemePicker;
        }

        private static void ThemePicker_SelectedIndexChanged(object? sender, EventArgs e)
        {
            Picker? picker = sender as Picker;
            if(picker != null)
            {
                GameStateService.TicTacToeTheme = Themes[picker.SelectedIndex];
            }
        }
    }
}
