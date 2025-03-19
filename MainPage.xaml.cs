using alexm_app.Utils.TicTacToe;

namespace alexm_app;

public partial class MainPage : ContentPage
{
    public List<string> Texts { get; set; } = new List<string> { "Tee lahti ValgusfoorPage", "Izitizer on the beat", "Lumememm", "Tic-Tac-Toe", "Friends", "Countries" };
    ScrollView ScrollContainer { get; set; }
    VerticalStackLayout MainContainer { get; set; }
    public MainPage()
    {
        GameStateService.Navigation = Navigation;
        Title = "Avaleht";
        MainContainer = new VerticalStackLayout { BackgroundColor = Color.FromArgb("#161824") };
        int index = -1;
        foreach (string text in Texts)
        {
            index++;
            Button button = new Button
            {
                Text = text,
                BackgroundColor = Color.FromArgb("#0b0d17"),
                TextColor = Color.FromArgb("#ffff"),
                BorderWidth = 10,
                ZIndex = index,
                FontFamily = "Skandal",
                FontSize = 25,
                WidthRequest = 350,
                Margin = new Thickness(0, 50, 0, 0)
            };
            MainContainer.Add(button);
            button.Clicked += Lehte_avamine;
        }
        ScrollContainer = new ScrollView { Content = MainContainer };
        Content = ScrollContainer;

    }

    private async void Lehte_avamine(object? sender, EventArgs e)
    {
        Button btn = (Button)sender;
        await Navigation.PushAsync(AppContext.Pages[btn.ZIndex]);
    }

    private async void Tagasi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}