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
    public ListView ListView { get; set; } = new ListView { RowHeight = 140 };

	public CountriesPage()
	{
		Debug.WriteLine("Starting");
        ListView.ItemsSource = ListItems;
        this.BackgroundColor = Color.FromArgb("#2b2b36");
        ListView.ItemTemplate = new DataTemplate(() =>
		{
			Index += 1;
            Image image = new Image { WidthRequest = 50, HeightRequest = 50 };
            
            image.SetBinding(Image.SourceProperty, new Binding("Png", converter: new ImagePathConverter()));

            Label label = new Label() { TextColor = Color.FromArgb("#ffffff"), VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
			label.SetBinding(Label.TextProperty, "Name");
            Label description = new Label() { TextColor = Color.FromArgb("#ffffff") };
            description.SetBinding(Label.TextProperty, "Description");
			Button changeFlagName = new Button { Text = "✏" };
			changeFlagName.SetBinding(Button.BindingContextProperty, "Name");
            changeFlagName.Clicked += ChangeFlagName_Clicked;
			Button deleteFlagButton = new Button { Text = "🗑", };
            deleteFlagButton.Clicked += DeleteFlagButton_Clicked;
            deleteFlagButton.SetBinding(Button.BindingContextProperty, "Name");
            Button changeFlag = new Button { Text = "🏴" };
            changeFlag.Clicked += ChangeFlag_Clicked;
            changeFlag.SetBinding(Button.BindingContextProperty, "Name");
            Button changeDescription = new Button() { Text = "📝" };
            changeDescription.Clicked += ChangeDescription_Clicked;
            changeDescription.SetBinding(Button.BindingContextProperty, "Name");

            Button moreButton = new Button() { Text = "📃" };
            moreButton.Clicked += MoreButton_Clicked;
            moreButton.SetBinding(Button.BindingContextProperty, "Name");
            StackLayout nameDescriptionContainer = new StackLayout()
            {
                Children = { label, description }
            };
            StackLayout buttonsContainer = new StackLayout()
            {
                Children = { changeFlagName, deleteFlagButton, changeFlag, changeDescription, moreButton },
                HorizontalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                Margin = new Thickness(0, 0, 0, 50)
            };
            TapGestureRecognizer tgr = new TapGestureRecognizer();
            tgr.Tapped += Tgr_Tapped;
            HorizontalStackLayout hContainer = new HorizontalStackLayout { Children = { image, nameDescriptionContainer } , Spacing = 50 };
            ViewCell ret = new ViewCell
            {
                View = new StackLayout
                {
                    BackgroundColor = Color.FromArgb("#192026"),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 20,

                    Orientation = StackOrientation.Vertical,
                    Children = { hContainer, buttonsContainer }
                }
            };
            image.GestureRecognizers.Add(tgr);
           
            return ret;
		});
        Debug.WriteLine("Calling");
        _ = LoadCountries();
	}

    private async void MoreButton_Clicked(object? sender, EventArgs e)
    {
        Button? button = sender as Button;
        if(button != null)
        {
            Debug.WriteLine("gg");
            string? name = button.BindingContext as string;
            if(name != null)
            {
                int index = GetIndexByName(name);
                Debug.WriteLine("gg");
                ForeignPage page = new ForeignPage(ListItems[index].Png, ListItems[index].Name, ListItems[index].Description);
                await this.Navigation.PushAsync(page);
            }
        }
    }

    private void Tgr_Tapped(object? sender, TappedEventArgs e)
    {
        Image? img = sender as Image;
        if(img != null)
        {
            string? name = img.BindingContext as string;
            if(name != null)
            {
                Debug.WriteLine(img);
            }
        }
    }

    private async void ChangeDescription_Clicked(object? sender, EventArgs e)
    {
        Button? button = sender as Button;
        if(button != null)
        {
            string? name = button.BindingContext as string;
            if (name != null)
            {
                string response = await DisplayPromptAsync("Prompt", "Enter the new description");
                int index = GetIndexByName(name);
                if (index != -1)
                {
                    ListItems[index].Description = response;
                }
            }
        }
    }

    private async void ChangeFlag_Clicked(object? sender, EventArgs e)
    {
        Button? button = sender as Button;
        if(button != null)
        {
            string? name = button.BindingContext as string;
            if(name != null)
            {
                FileResult? fileResponse = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Vali pilt", FileTypes = FilePickerFileType.Images });

                if (fileResponse != null)
                {
                    string destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileResponse.FileName);
                    using (var stream = await fileResponse.OpenReadAsync())
                    using (var newStream = File.Create(destinationPath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    int index = GetIndexByName(name);
                    if (index != -1)
                    {
                        ListItems[index].Png = fileResponse.FileName;
                    }
                }
            }
        }
    }

    private bool IsThereName(string name)
    {
        foreach(ListItem li in ListItems)
        {
            if (li.Name == name)
            {
                return true;
            }
        }
        return false;
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
                if(!IsThereName(response))
                if (response != null) ChangeName(name, response);

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
        if (item != null) ListItems.Remove(item);
        
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
        if (IsThereName(response))
        {
            await this.DisplayAlert("Alert", "This name is already in list", "OK");
            return;
        }
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
            Countries.Add(newCountry);
            ListItems.Add(new ListItem { Png = fileResponse.FileName, Name = response });
        }
    }

}
public class ListItem : INotifyPropertyChanged
{
    private string _png;
    private string _name;
    private string _description;

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
    public string Description
    {
        get 
        {
            if (_description != null)
                return _description;
            else
                return "";
        }
        set
        {
            if(_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
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