using alexm_app.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace alexm_app.Pages.Countries;

public partial class CountriesPage : ContentPage
{
	public List<Country> Countries { get; set; }
	public bool Loaded { get; set; } = false;
	public ListView ListView { get; set; } = new ListView { RowHeight = 80 };	
	public CountriesPage()
	{
		Debug.WriteLine("Starting");
		ListView.ItemTemplate = new DataTemplate(() =>
		{
			Image image = new Image { WidthRequest = 50, HeightRequest = 50 };
			image.SetBinding(Image.SourceProperty, "Png");
			Label label = new Label();
			label.SetBinding(Label.TextProperty, "Name");
			return new ViewCell { View = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = {image, label}
			}
			};
		});
        Debug.WriteLine("Calling");
        _ = LoadCountries();
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
        List<ListItem> list = new List<ListItem>();
		using(HttpClient client = new HttpClient())
		{
            foreach (var country in Countries)
            {
				Debug.WriteLine(country.Flags.Png);

                var bytes = await client.GetByteArrayAsync(country.Flags.Png);
                Uri uri = new Uri(country.Flags.Png);
                string fileName = Path.GetFileName(uri.LocalPath);
				await File.WriteAllBytesAsync(Path.Combine(FileSystem.AppDataDirectory, fileName), bytes);
                Debug.WriteLine($"country: {country.Name.Common}\n");

                list.Add(new ListItem() { Png = fileName, Name = country.Name.Common });
            }
        }

		this.ListView.ItemsSource = list;
		Content = new StackLayout { Children = { ListView } };
    }
}
public class ListItem
{
	public string Png { get; set; }
	public string Name { get; set; }
}