using alexm_app.Models;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace alexm_app.Pages.Countries;

public partial class CountriesPage : ContentPage
{
	public List<Country> Countries { get; set; }
	public bool Loaded { get; set; } = false;
	public int Index { get; set; } = -1;
    public ObservableCollection<ListItem> ListItems = new ObservableCollection<ListItem>();
    public ListView ListView { get; set; } = new ListView { RowHeight = 80 };

	public CountriesPage()
	{
		Debug.WriteLine("Starting");
        ListView.ItemsSource = ListItems;

        ListView.ItemTemplate = new DataTemplate(() =>
		{
			Index += 1;
            Image image = new Image { WidthRequest = 50, HeightRequest = 50 };
            image.SetBinding(Image.SourceProperty, new Binding("Png", converter: new ImagePathConverter()));
            
            Label label = new Label() { TextColor = Color.FromArgb("#ffffff"), VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
			label.SetBinding(Label.TextProperty, "Name");
			Button changeFlagName = new Button { Text = "✏" };
			changeFlagName.SetBinding(Button.BindingContextProperty, "Name");
            changeFlagName.Clicked += ChangeFlagName_Clicked;
			Button deleteFlagButton = new Button { Text = "🗑", };
            deleteFlagButton.Clicked += DeleteFlagButton_Clicked;
            
            return new ViewCell { View = new StackLayout
			{
				BackgroundColor = Color.FromArgb("#192026"),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = this.WidthRequest,
				

				Orientation = StackOrientation.Horizontal,
				Children = { image, label, changeFlagName, deleteFlagButton }
			}
			};
		});
        Debug.WriteLine("Calling");
        _ = LoadCountries();
	}

    private async void ChangeFlagName_Clicked(object? sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            string? name = button.BindingContext as string;
            if (name != null)
            {
                string? response = await this.DisplayPromptAsync("Prompt", "Enter the new name");
                if(response != null) ChangeName(name, response);

            }
        }
    }

    private async void DeleteFlagButton_Clicked(object? sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            string? name = button.BindingContext as string;
            if(name != null)
            {
                RemoveFromCollection(name);
            }
        }
    }

    private void ChangeName(string name, string newName)
    {
        int index = GetIndexByName(name);
        if (index != -1) ListItems[index].Name = newName;
    }


    private void RemoveFromCollection(string name)
    {
        var item = ListItems.FirstOrDefault(c => c.Name == name);
        if (item != null)
        {
            ListItems.Remove(item);
        }
    }

    private int GetIndexByName(string name)
    {
        int index = -1;
        foreach (ListItem cntry in ListItems)
        {
            index++;
            if (cntry.Name == name)
            {
                break;
            }
        }
        return index;
    }
    private async Task LoadCountries()
	{
        Debug.WriteLine("Called");
        using (HttpClient client = new HttpClient())
		{
			HttpResponseMessage response = await client.GetAsync(@"https://restcountries.com/v3.1/all");
			if (response.IsSuccessStatusCode)
			{
				string strResponse = await response.Content.ReadAsStringAsync();
                List<Country>? countries = JsonConvert.DeserializeObject<List<Country>>(strResponse);
				if(countries != null)
				{
					Countries = countries;
					Loaded = true;
                    Debug.WriteLine("OnLoaded");
                    OnLoaded();
                }
			}
		}
	}
	public async Task OnLoaded()
	{
        Debug.WriteLine("OnLoaded called");
        using (HttpClient client = new HttpClient())
		{
            foreach (var country in Countries)
            {;

                var bytes = await client.GetByteArrayAsync(country.Flags.Png);
                Uri uri = new Uri(country.Flags.Png);
                string fileName = Path.GetFileName(uri.LocalPath);
       
                await File.WriteAllBytesAsync(Path.Combine(FileSystem.AppDataDirectory, fileName), bytes);

                ListItems.Add(new ListItem() { Png = fileName, Name = country.Name.Common });
            }
        }

		
		Button button = new Button() { Text="Lisa riigid"};
        button.Clicked += Button_Clicked;
		Content = new StackLayout { Children = { button, ListView } };
    }

    private async void Button_Clicked(object? sender, EventArgs e)
    {
        string response = await this.DisplayPromptAsync("Prompt", "Enter the name");
        FileResult? fileResponse = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Vali pilt", FileTypes = FilePickerFileType.Images });

        if (fileResponse != null)
        {
            string destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileResponse.FileName);
            using (var stream = await fileResponse.OpenReadAsync())
            using (var newStream = File.Create(destinationPath))
            {
                await stream.CopyToAsync(newStream);
            }

            var newCountry = new Country { Flags = new Flags { Png = fileResponse.FileName }, Name = new Name { Common = response } };
            Countries.Add(newCountry); // Добавляем в основной список

            // Добавляем в ObservableCollection, чтобы UI обновился
            ListItems.Add(new ListItem { Png = fileResponse.FileName, Name = response });
        }
    }

}
public class ListItem : INotifyPropertyChanged
{
    private string _png;
    private string _name;

    public string Png
    {
        get => _png;
        set
        {
            if (_png != value)
            {
                _png = value;
                OnPropertyChanged(nameof(Png));
            }
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ImagePathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string fileName)
        {

            return Path.Combine(FileSystem.AppDataDirectory, fileName);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}