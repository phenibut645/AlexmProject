using alexm_app.Services;
using alexm_app.Utils.TicTacToe;
using System.Diagnostics;

namespace alexm_app.Pages.TicTacToe.GuestPages;

public partial class CreateRoom : ContentPage
{
	public VerticalStackLayout MainContainer { get; set; } = new VerticalStackLayout();
	public HorizontalStackLayout RoomNameContainer { get; set; } = new HorizontalStackLayout();
	public Label RoomNameLabel { get; set; } = new Label() { Text = "Room name"};
	public Entry RoomNameEntry { get; set; } = new Entry();
	public VerticalStackLayout AreaSizeMultiplyContainer { get; set; } = new VerticalStackLayout();
	public Label AreaSizeMultiplyLabel { get; set; } = new Label() { Text = "How many cells in rows and columns: 1"};
	public Slider AreaSizeMultiplySlider { get; set; } = new Slider() { Minimum = 1, Maximum = 5 };
	public Button CreateButton { get; set; } = new Button() { Text = "Create" };

	public int Value { get; set; } = 1;
	
	public CreateRoom()
	{
		Content = MainContainer;
		RoomNameContainer.Children.Add(RoomNameLabel);
		RoomNameContainer.Children.Add(RoomNameEntry);
		AreaSizeMultiplyContainer.Children.Add(AreaSizeMultiplyLabel);
		AreaSizeMultiplyContainer.Children.Add(AreaSizeMultiplySlider);
		MainContainer.Children.Add(RoomNameContainer);
		MainContainer.Children.Add(AreaSizeMultiplyContainer);
		MainContainer.Children.Add(CreateButton);
        AreaSizeMultiplySlider.ValueChanged += AreaSizeMultiplySlider_ValueChanged;
        CreateButton.Clicked += CreateButton_Clicked;
	}

    private async void CreateButton_Clicked(object? sender, EventArgs e)
    {
        if(RoomNameEntry.Text == "")
		{
			await this.DisplayAlert("Error", "Type the name of room", "ok");
			return;
		}
		else if (!(await DatabaseHandler.IsRoomNameAvailable(RoomNameEntry.Text)))
		{
			await this.DisplayAlert("Error", "This name isn't available", "ok");
			return;
		}
		if(GameStateService.Username == null) return;
		Debug.WriteLine("Creating game...");
		await MultiplayerHandler.CreateRoom(GameStateService.Username, RoomNameEntry.Text, Value);	
    }

    private void AreaSizeMultiplySlider_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        Slider? slider = sender as Slider;
		if(slider != null)
		{
			int value = (int)Math.Round(slider.Value);
			Value = value;
			AreaSizeMultiplyLabel.Text = $"How many cells in rows and columns: {Value}";
		}
    }
}