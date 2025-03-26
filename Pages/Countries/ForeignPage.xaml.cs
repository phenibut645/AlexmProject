using Microsoft.Maui.Storage;

namespace alexm_app.Pages.Countries;

public partial class ForeignPage : ContentPage
{
	private VerticalStackLayout MainContainer = new VerticalStackLayout() { Spacing = 20 };
	public ForeignPage(string imageName, string name, string description)
    {
		Image image = new Image() { Source = Path.Combine(FileSystem.AppDataDirectory, imageName), WidthRequest=60 };
		Label flagName = new Label() { Text = name, FontSize = 20 };
		Label descriptionLabel = new Label() { Text = description, FontSize = 15 };
		Content = MainContainer;
		MainContainer.Children.Add(image);
		MainContainer.Children.Add(flagName);
		MainContainer.Children.Add(descriptionLabel);
	}
}