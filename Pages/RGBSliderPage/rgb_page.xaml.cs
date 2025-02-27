using alexm_app.Models;

namespace alexm_app;

public partial class rgb_page : ContentPage
{
    public VerticalStackLayout MainContainer { get; set; }
    public VerticalStackLayout SlidersContainer { get; set; } 
    public Button RGBColorView { get; set; } = new Button()
    {
        HeightRequest = 200,
        WidthRequest = 0,
    };
    public Label RGBlabel { get; set; }
    public Button RandomRGBValuesButton { get; set; } = new Button
    {
        FontSize = 15,
        Text = "Random RGB"
    };

    public RGBSliders RGBSliders { get; set; } = new RGBSliders();
    public Slider WidthSlider { get; set; } = new Slider() {
        Minimum = 0,
        Maximum = 200
    };
    public Entry hex { get; set; } = new Entry()
    {
        MaxLength = 7,
        Text = "#"
    };
   
    public rgb_page()
    {
        MainContainer = new VerticalStackLayout()
        {
            BackgroundColor = Color.FromArgb("#0b0d17")

        };
        SlidersContainer = new VerticalStackLayout()
        {
            BackgroundColor = Color.FromArgb("#121526"),
            HeightRequest = 400,
            Spacing = 50,
            Padding = 50
        };
        Content = this.MainContainer;
        MainContainer.Children.Add(RGBColorView);
        MainContainer.Children.Add(SlidersContainer);
        foreach (Slider slider in RGBSliders.SlidersList)
        {
            SlidersContainer.Children.Add(slider);
        }
        RGBSliders.FinalColorChanged += FinalColorChanged;
        RGBlabel = new Label()
        {
            FontSize = 22
        };
        MainContainer.Children.Add(RGBlabel);
        RGBColorView.Clicked += RGBColorView_Clicked;
        MainContainer.Children.Add(RandomRGBValuesButton);
        RandomRGBValuesButton.Clicked += RandomRGBValuesButton_Clicked;
        SlidersContainer.Add(WidthSlider);
        WidthSlider.ValueChanged += WidthSlider_ValueChanged;
        MainContainer.Children.Add(hex);
        hex.TextChanged += Hex_TextChanged;
    }

    private void Hex_TextChanged(object? sender, TextChangedEventArgs e)
    {
        Entry entry = sender as Entry;
        try
        {
            if (entry != null)
            {
                RGBColorView.BackgroundColor = Color.FromArgb(entry.Text);

            }
        }
        catch (Exception err)
        {

        }
    }

    private void WidthSlider_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        Slider? slider = sender as Slider;
        RGBColorView.WidthRequest = slider!.Value;

    }

    private void RandomRGBValuesButton_Clicked(object? sender, EventArgs e)
    {
        Random rand = new Random();
        RGBSliders.Red = rand.Next(255);
        RGBSliders.Green = rand.Next(255);
        RGBSliders.Blue = rand.Next(255);
    }

    private async void RGBColorView_Clicked(object? sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync($"{RGBSliders.FinalColor[0]}, {RGBSliders.FinalColor[1]}, {RGBSliders.FinalColor[2]}");
        await DisplayAlert("Clipboard", $"{RGBSliders.FinalColor[0]}, {RGBSliders.FinalColor[1]}, {RGBSliders.FinalColor[2]}", "OK");
    }

    private void FinalColorChanged(List<int> rgb)
    {
        RGBColorView.BackgroundColor = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
        RGBlabel.Text = $"Red: {rgb[0]}, Green: {rgb[1]}, Blue: {rgb[2]}";
        RandomRGBValuesButton.BackgroundColor = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
    }
    
}