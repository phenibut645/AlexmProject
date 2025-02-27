using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace alexm_app
{
    public partial class lumimamm : ContentPage
    {
        private AbsoluteLayout SnowmanLayout { get; set; }
        private BoxView Bucket { get; set; }
        private Ellipse head, body;
        private Label ActionLabel { get; set; }
        private Random rnd { get; set; } = new Random();
        private Image Mask { get; set;  }
        private Image Hair { get; set; } = new Image() { Source = ImageSource.FromFile("zxchair.png"), WidthRequest = 120, HeightRequest = 120 };
        private Image Midas { get; set; }
        private Image Nix { get; set; } = new Image { Source = ImageSource.FromFile("serega.png"), Opacity = 1, WidthRequest = 150, HeightRequest = 150, IsAnimationPlaying = true };
        private Image ZxcEgor { get; set; } = new Image { Source = ImageSource.FromFile("zxcegor.gif"), WidthRequest = 100, HeightRequest = 100, IsAnimationPlaying = true, Opacity = 0 };
        private Image Zavtra { get; set;  } = new Image { Source = ImageSource.FromFile("zaftra.gif"), WidthRequest = 140, HeightRequest = 140, IsAnimationPlaying = true, Opacity = 0 };

        public lumimamm()
        {
            SnowmanLayout = new AbsoluteLayout();

            head = new Ellipse { Fill = Colors.White, WidthRequest = 60, HeightRequest = 60 };
            body = new Ellipse { Fill = Colors.White, WidthRequest = 80, HeightRequest = 80 };
            Mask = new Image { Source = ImageSource.FromFile("mask_of_madness.png"), WidthRequest = 70, HeightRequest = 70 };
            Midas = new Image { Source = ImageSource.FromFile("midas.png"), WidthRequest = 60, HeightRequest = 60 , Rotation = 90};
            AbsoluteLayout.SetLayoutBounds(head, new Rect(70, 50, 60, 60));
            AbsoluteLayout.SetLayoutBounds(body, new Rect(60, 110, 80, 80));
            AbsoluteLayout.SetLayoutBounds(Mask, new Rect(70, 50, 60, 60));
            AbsoluteLayout.SetLayoutBounds(Midas, new Rect(90, 110, 60, 60));
            AbsoluteLayout.SetLayoutBounds(Hair, new Rect(75, 50, 50, 40));
            AbsoluteLayout.SetLayoutBounds(Zavtra, new Rect(150, 50, 150, 150));
            

            
            SnowmanLayout.Children.Add(head);
            SnowmanLayout.Children.Add(body);
            SnowmanLayout.Children.Add(Mask);
            SnowmanLayout.Children.Add(Midas);
            SnowmanLayout.Children.Add(Hair);
            SnowmanLayout.Children.Add(Zavtra);
            

            ActionLabel = new Label { Text = "Vali tegevus", FontSize = 18, HorizontalOptions = LayoutOptions.Center, FontFamily = "DKCinnabarBrush-Regular" };

            Button hideButton = new Button { Text = "Peida", FontFamily = "DKCinnabarBrush-Regular", BackgroundColor = Color.FromArgb("#292929"), TextColor = Color.FromArgb("#e8e8e8") };
            hideButton.Clicked += (s, e) => ToggleVisibility(false);

            Button showButton = new Button { Text = "Näita", FontFamily = "DKCinnabarBrush-Regular", BackgroundColor = Color.FromArgb("#292929"), TextColor = Color.FromArgb("#e8e8e8") };
            showButton.Clicked += (s, e) => ToggleVisibility(true);

            Button colorButton = new Button { Text = "Värvi muuda", FontFamily = "DKCinnabarBrush-Regular", BackgroundColor = Color.FromArgb("#292929"), TextColor = Color.FromArgb("#e8e8e8") };
            colorButton.Clicked += async (s, e) => await ChangeColor();

            Slider meltSlider = new Slider { Minimum = 0, Maximum = 1, Value = 1 };
            meltSlider.ValueChanged += (s, e) => MeltSnowman(e.NewValue);

            Button rotateButton = new Button { Text = "Pöörata", FontFamily = "DKCinnabarBrush-Regular", BackgroundColor = Color.FromArgb("#292929"), TextColor = Color.FromArgb("#e8e8e8") };
            rotateButton.Clicked += (s, e) => RotateSnowman();

            StackLayout controlsLayout = new StackLayout
            {
                Children = { hideButton, showButton, colorButton, meltSlider, rotateButton },
                Padding = 10
            };

            Content = new StackLayout
            {
                Children = { ActionLabel, SnowmanLayout, controlsLayout, Nix, ZxcEgor }
            };
            (Content as StackLayout)!.BackgroundColor = Color.FromArgb("#c2c2c2");
        }

        private void ToggleVisibility(bool isVisible)
        {
            Hair.IsVisible = isVisible;
            head.IsVisible = isVisible;
            body.IsVisible = isVisible;
            Mask.IsVisible = isVisible;
            Midas.IsVisible = isVisible;
            Zavtra.IsVisible = isVisible;
            ActionLabel.Text = isVisible ? "Lumimam on näitud" : "Lumimam on peidatud";
        }

        private async Task ChangeColor()
        {
            int r = rnd.Next(0, 255);
            int g = rnd.Next(0, 255);
            int b = rnd.Next(0, 255);
            bool confirm = await DisplayAlert("Värvi muutamine", $"Muuda värv RGB'ks ({r}, {g}, {b})?", "Jah", "Ei");
            if (confirm)
            {
                head.Fill = Color.FromRgb(r, g, b);
                body.Fill = Color.FromRgb(r, g, b);
                ActionLabel.Text = "Värv oli muutanud";
            }
        }

        private void MeltSnowman(double value)
        {
            SnowmanLayout.Opacity = value;
            ActionLabel.Text = "Oh eii...";
            Zavtra.Opacity = 1 - value;
            if (value < 0.7)
            {
                Nix.Source = ImageSource.FromFile("nix_vax.png");
                ZxcEgor.Opacity = 1;
                Mask.Source = ImageSource.FromFile("rostik.png");
            }
            else
            {

                Nix.Source = ImageSource.FromFile("serega.gif");
                ZxcEgor.Opacity = 0;
                Mask.Source = ImageSource.FromFile("mask_of_madness.png");
            }
        }

        private void RotateSnowman()
        {
            SnowmanLayout.RotateTo(SnowmanLayout.Rotation + 45, 500);
            ActionLabel.Text = "hell yeah";
        }
    }
}
