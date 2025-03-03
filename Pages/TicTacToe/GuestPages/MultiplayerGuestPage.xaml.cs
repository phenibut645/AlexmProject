using alexm_app.Models.TicTacToe.ServerMessages;
using alexm_app.Services;
using alexm_app.Utils.TicTacToe;
using System.Diagnostics;
using alexm_app.Pages.TicTacToe.GuestPages;

namespace alexm_app;

public partial class MultiplayerGuestPage : ContentPage
{
	public VerticalStackLayout MainContainer { get; set; } = new VerticalStackLayout() { HorizontalOptions = LayoutOptions.Center, Spacing = 40};
	public VerticalStackLayout PickerContainer { get; set; } = new VerticalStackLayout();
	public HorizontalStackLayout PickerEntryContainer { get;set; } = new HorizontalStackLayout();
	public Label RoomPickerLabel { get; set; } = new Label() { Text="Rooms"};
	public Picker RoomPicker { get; set;} = new Picker();
	public Button CreateRoomButton { get; set; } = new Button() { Text="Create room"};
	public HorizontalStackLayout ButtonsContainer { get; set; } = new HorizontalStackLayout();
	public VerticalStackLayout UsernameEntryContainer { get; set; } = new VerticalStackLayout() { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start };
	public Label UsernameEntryLabel { get; set; } = new Label() { Text = "Username" };
	public Entry UsernameEntry { get; set; } = new Entry();
	public Button JoinRoomButton { get; set; } = new Button() { Text = "Join room" };
	public Button RefreshPicker { get; set; } = new Button() { Text="Refresh"};
	public Button RejoinButton { get; set; } = new Button() { IsVisible = false, Text = "Rejoin"};
	public List<AvailableGame>? Games { get; set; }
	public MultiplayerGuestPage()
	{
		Content = MainContainer;
		MainContainer.Children.Add(PickerContainer);
		PickerContainer.Children.Add(RoomPickerLabel);
		PickerContainer.Children.Add(PickerEntryContainer);

		PickerEntryContainer.Children.Add(RoomPicker);
		PickerEntryContainer.Children.Add(RefreshPicker);
		
		MainContainer.Children.Add(UsernameEntryContainer);
		UsernameEntryContainer.Children.Add(UsernameEntryLabel);
		UsernameEntryContainer.Children.Add(UsernameEntry);
		ButtonsContainer.Children.Add(JoinRoomButton);
		ButtonsContainer.Children.Add(CreateRoomButton);
		MainContainer.Children.Add(ButtonsContainer);
		MainContainer.Children.Add(RejoinButton);
		MainContainer.Children.Add(ReportSender.GetReportButton());
        _ = InitPickerItems();
        InitEventListeners();
        MultiplayerHandler.OnRunningGameClose += MultiplayerHandler_OnRunningGameClose;
	}

    private void MultiplayerHandler_OnRunningGameClose()
    {
        RejoinButton.IsVisible = true;
		MultiplayerHandler.Rejoin();
    }

    private void InitEventListeners()
	{
		RefreshPicker.Clicked += RefreshPicker_Clicked;
        JoinRoomButton.Clicked += async (object? sender, EventArgs e) =>
		{
			if(!(await DatabaseHandler.IsUsernameAvailable(UsernameEntry.Text)))
			{
				await DisplayAlert("Alert", "This username isn't available!", "OK");
				return;
			}
			if(RoomPicker.SelectedIndex == -1 || UsernameEntry.Text == "") await DisplayAlert("Alert", "Some entries is empty!", "OK");
			else if(Games == null) await DisplayAlert("Alert", "There's no available room", "OK");
			else await MultiplayerHandler.JoinPlayer(UsernameEntry.Text, Games[RoomPicker.SelectedIndex]);
		};
		CreateRoomButton.Clicked += async (object? sender, EventArgs e) =>
		{
			if(UsernameEntry.Text == "") await DisplayAlert("Alert", "Some entries is empty!", "OK");
			else if(!(await DatabaseHandler.IsUsernameAvailable(UsernameEntry.Text)))
			{
				await DisplayAlert("Alert", "This username isn't available!", "OK");
			}
			else
			{
				GameStateService.Username = UsernameEntry.Text;
				await Navigation.PushAsync(new CreateRoom());
			}
		};
	}

    private void RefreshPicker_Clicked(object? sender, EventArgs e)
    {
		RoomPicker.ItemsSource = new List<string>();
        _ = InitPickerItems();
    }

    public async Task InitPickerItems()
	{
		List<AvailableGame>? games = await DatabaseHandler.GetAvailableGames();
		Games = games;
		List<string> values = new List<string>();
		if(games != null)
		{
			foreach(AvailableGame game in games)
			{
				Debug.WriteLine($"{game.RoomName} (Player: {game.Username})");
				values.Add($"{game.RoomName} (Player: {game.Username})");
			}
		}
		RoomPicker.ItemsSource = values;
	}
}