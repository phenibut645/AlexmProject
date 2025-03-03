using alexm_app.Models;
using alexm_app.Services;
using alexm_app.Utils.TicTacToe;

namespace alexm_app;

public partial class GuestPage : ContentPage
{
	// TODO:
	// выбор размера игрового поля
	// кнопка одиночной игры, которая перекидывает на страницу где есть кнопка где можно вдвоем играть на одном компе, и вторая кнопка игра с ботом
	// кнопка мультиплеера с селектом выбора комнат или создания новой комнаты
	// выбор темы страницы где будет происходить крестики нолики
	// замена крестиков и ноликов на картинки
	// правила добавить как label
	// при игре в одиночную игру, нужен выбор того, кто ходит первый.

	public VerticalStackLayout MainContainer = new VerticalStackLayout();
	public Picker ThemePicker;
	
	public Button SinglePlayerButton = new Button() { Text = "Single player" };
	public Button MultiplayerButton = new Button() { Text = "Multiplayer" };
	public Label SizeOfGameAreaLabel = new Label() { Text = "Size of game area"};
	public Button? ReconnectButton = null;
	public Slider SizeOfGameAreaSlider = new Slider() { Maximum = 3, Minimum = 1 };
	public Image HeaderGif = new Image() { Source = ImageSource.FromFile("serega.gif"), IsAnimationPlaying = true, WidthRequest = 150, HeightRequest = 150 };
	
	public GuestPage()
	{
		this.Content = MainContainer;
		ThemePicker = GameService.GetThemePicker();
		List<IView> componentChronology = new List<IView>() { HeaderGif, ThemePicker, SizeOfGameAreaLabel, SizeOfGameAreaSlider, SinglePlayerButton, MultiplayerButton };
		foreach (IView component in componentChronology) MainContainer.Children.Add(component);
		InitEventListeners();
		InitRejoinButton();

		
		//Button button = MultiplayerHandler.GetReportButton(this);
		//MainContainer.Children.Add(button);

    }
	private void InitRejoinButton()
	{

	}
	private void InitEventListeners()
	{
        this.NavigatedFrom += GuestPage_NavigatedFrom;
		SinglePlayerButton.Clicked += async (object? sender, EventArgs args) =>
        {
			await Navigation.PushAsync(new SingleplayerGuestPage());
        };
		MultiplayerButton.Clicked += async (object? sender, EventArgs args) =>
		{
			await Navigation.PushAsync(new MultiplayerGuestPage());
		};
	}

    private void GuestPage_NavigatedFrom(object? sender, NavigatedFromEventArgs e)
    {
        onPageLeave();
    }

	private void onPageLeave()
	{
		GameStateService.TicTacToeTheme = GameService.Themes[ThemePicker.SelectedIndex];
		GameStateService.TicTacToeSizeOfMapMultiply = SizeOfGameAreaSlider.Value;
	}
}