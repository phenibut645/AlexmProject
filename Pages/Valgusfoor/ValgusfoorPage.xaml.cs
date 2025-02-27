using alexm_app.Models;

namespace alexm_app
{
    public partial class ValgusfoorPage : ContentPage
    {
        public bool IsItActive { get; set; } = true;
        public Dictionary<string, EventHandler> BottomButtons { get; set; }
        private bool _isCycleRunning = false;
        private CancellationTokenSource _cancellationTokenSource;
        public VerticalStackLayout MainContainer { get; set; } = new VerticalStackLayout()
        {
            BackgroundColor = Color.FromArgb("#0b0d17"),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill
        };
        public VerticalStackLayout BottomContainer { get; set; } = new VerticalStackLayout()
        {

            VerticalOptions = LayoutOptions.End,
            Spacing = 10,
            BackgroundColor = Color.FromArgb("#121526"),
            HeightRequest = 200,

        };
        public List<TrafficLightState> States { get; set; } = new List<TrafficLightState>()
        {
            new TrafficLightState("#ff0000", "#360000", "Seis!"),
            new TrafficLightState("#fffb00", "#242300", "Oota!"),
            new TrafficLightState("#11ff00", "#055200", "Mine!")
        };
        public VerticalStackLayout TrafficLightContainer { get; set; } = new VerticalStackLayout()
        {
            Spacing = 20
        };
        public Label StateTextLabel { get; set; } = new Label()
        {
            FontSize = 22,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#121526"),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 40)
        };
        public TrafficLightState ActiveState { get; set; }
        public void InitTrafficLightsButtons()
        {
            List<Button> buttons = new List<Button>();

            foreach (TrafficLightState state in States)
            {

                Button stateButton = new Button()
                {
                    BackgroundColor = state.CurrentColor,
                    WidthRequest = 100,
                    HeightRequest = 100,
                    CornerRadius = 50
                };
                TrafficLightContainer.Children.Add(stateButton);
                state.Button = stateButton;
                state.ActiveChanged += State_ActiveChanged;
                stateButton.Clicked += (object? sender, EventArgs e) =>
                {
                    //_cancellationTokenSource?.Cancel();
                   // _isCycleRunning = false;

                    if (IsItActive)
                    {
                        TrafficLightState tls = state;
                        if (ActiveState != null) ActiveState.Active = false;
                        tls.Active = !tls.Active;
                        ActiveState = tls;
                    }
                };
            }

        }
        private void InitButtons()
        {
            int count = 0;
            HorizontalStackLayout currentContainer = new HorizontalStackLayout()
            {
                BackgroundColor = Color.FromArgb("#121526")
            };
            foreach (KeyValuePair<string, EventHandler> bottomButtonProps in BottomButtons)
            {
                if (count == 0)
                {
                    BottomContainer.Children.Add(currentContainer);
                }


                Button button = new Button
                {
                    Text = bottomButtonProps.Key,
                    WidthRequest = 150,
                    TextColor = Colors.White,
                    FontSize = 18,
                    HeightRequest = 50,
                    BackgroundColor = Color.FromArgb("#0b0d17")
                };
                button.Clicked += bottomButtonProps.Value;
                currentContainer.Children.Add(button);
                count++;
                if (count == 2)
                {
                    currentContainer = new HorizontalStackLayout()
                    {
                        BackgroundColor = Color.FromArgb("#121526")
                    };
                    count = 0;
                }
            }

        }
        private void TurnOff(object? sender, EventArgs e)
        {
            IsItActive = false;
            //_cancellationTokenSource?.Cancel();
           // _isCycleRunning = false;

            foreach (TrafficLightState tls in States)
            {
                tls.Active = false;
            }
        }
        private void TurnOn(object? sender, EventArgs e)
        {
            IsItActive = true;
        }
        private void State_ActiveChanged(TrafficLightState trafficLightState)
        {
            if (!trafficLightState.Active) StateTextLabel.Text = "";
            else StateTextLabel.Text = trafficLightState.Text;
        }

        private async void AutoMode(object? sender, EventArgs e)
        {
            if (_isCycleRunning)
            {
                _cancellationTokenSource.Cancel();
                _isCycleRunning = false;
                TurnOff(null, new EventArgs());
                IsItActive = true;
            }
            else
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _isCycleRunning = true;
                TurnOff(null, new EventArgs());
                IsItActive = true;
                await StartAutoMode();
            }
        }
        private async Task StartAutoMode()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                foreach (TrafficLightState state in States)
                {
                    state.Active = true;
                    await Task.Delay(1000);
                    state.Active = false;
                    if (_cancellationTokenSource.IsCancellationRequested) return;
                }
            }
        }
        private async void NightMode(object? sender, EventArgs e)
        {
            if (_isCycleRunning)
            {
                _cancellationTokenSource.Cancel();
                _isCycleRunning = false;
                TurnOff(null, new EventArgs());
                IsItActive = true;
            }
            else
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _isCycleRunning = true;
                TurnOff(null, new EventArgs());
                IsItActive = true;
                await StartNightMode();
            }
        }
        private async Task StartNightMode()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                States[1].Active = true;
                await Task.Delay(1000);
                States[1].Active = false;
                await Task.Delay(1000);
            }
        }
        public ValgusfoorPage()
        {
            BottomButtons = new Dictionary<string, EventHandler>()
            {
                { "Lülita välja",  TurnOff },
                { "Lülita sisse", TurnOn},
                { "Automaatreþiimi sisselülitamine", AutoMode },
                { "Kollane",  NightMode}
            };
            Content = MainContainer;

            MainContainer.Children.Add(StateTextLabel);
            MainContainer.Children.Add(TrafficLightContainer);
            MainContainer.Children.Add(BottomContainer);
            InitTrafficLightsButtons();
            InitButtons();
        }

    }
}