using alexm_app.Enums.TicTacToe;
using static alexm_app.TicTacToePage;

namespace alexm_app;

public partial class SingleplayerGuestPage : ContentPage
{
	public VerticalStackLayout MainContainer { get; set; } = new VerticalStackLayout();
	public Button BotGame { get; set; } = new Button { Text = "Game with bots" };
	public Button Singleplayer { get; set; } = new Button { Text = "One computer game"};
	public Image DeadInside { get; set; } = new Image { Source = ImageSource.FromFile("zaftra.gif"), IsAnimationPlaying = true, HeightRequest = 100};
	public Picker SidePicker { get; set; } = new Picker() { ItemsSource = new List<string>() { "Cross", "Nought"} };
	public List<IView> ComponentChronology
	{
		get
		{
			return new List<IView>() { DeadInside, BotGame, Singleplayer };
		}
	}
	
	public SingleplayerGuestPage()
	{
		Content = MainContainer;
		foreach(IView component in ComponentChronology) MainContainer.Children.Add(component);
		InitEventListeners();
	} 
	private void InitEventListeners()
	{
		BotGame.Clicked += async (object? sender, EventArgs args) => await Navigation.PushAsync(new TicTacToePage(async () => { }, 1));
		Singleplayer.Clicked += async (object? sender, EventArgs args) => await Navigation.PushAsync(new TicTacToePage(async () => { }, 1));
	}
}