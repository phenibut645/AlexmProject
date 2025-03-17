using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace alexm_app.Pages.Friends
{
    public class Friend
    {
        public string Name { get; set; }
        public string Photo { get; set; } = "default_photo.png";
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string Message { get; set; } = "";
    }

    public class FriendsPage : ContentPage
    {
        private ObservableCollection<Friend> friends;
        private Random random = new Random();
        private readonly string[] greetings =
        {
            "Häid pühi!",
            "Palju õnne!",
            "Parimad soovid!",
            "Tervist ja rõõmu!",
            "Õnnistatud päeva!"
        };

        private ListView friendsListView;

        public FriendsPage()
        {
            friends = new ObservableCollection<Friend>
            {
                new Friend { Name = "Karl", Email = "karl@example.com", Phone = "12345678", Description = "Sõber" },
                new Friend { Name = "Anna", Email = "anna@example.com", Phone = "87654321", Description = "Parim sõber" }
            };

            friendsListView = new ListView
            {
                ItemsSource = friends,
                ItemTemplate = new DataTemplate(() =>
                {
                    var photo = new Image { HeightRequest = 50, WidthRequest = 50 };
                    photo.SetBinding(Image.SourceProperty, "Photo");

                    var nameLabel = new Label { FontSize = 20 };
                    nameLabel.SetBinding(Label.TextProperty, "Name");

                    var descriptionLabel = new Label { FontSize = 14 };
                    descriptionLabel.SetBinding(Label.TextProperty, "Description");

                    var messageEntry = new Entry { Placeholder = "Sisesta sõnum" };
                    messageEntry.SetBinding(Entry.TextProperty, "Message");

                    var callButton = new Button { Text = "Helista" };
                    callButton.SetBinding(Button.CommandParameterProperty, "Phone");
                    callButton.Clicked += (s, e) =>
                    {
                        var button = (Button)s;
                        var phone = button.CommandParameter as string;
                        if (!string.IsNullOrEmpty(phone))
                            PhoneDialer.Open(phone);
                    };

                    var smsButton = new Button { Text = "Saada SMS" };
                    smsButton.SetBinding(Button.CommandParameterProperty, "Phone");
                    smsButton.Clicked += (s, e) =>
                    {
                        var button = (Button)s;
                        var phone = button.CommandParameter as string;
                        var friend = (Friend)button.BindingContext;
                        Sms.ComposeAsync(new SmsMessage(friend.Message, phone));
                    };

                    var emailButton = new Button { Text = "Saada Email" };
                    emailButton.SetBinding(Button.CommandParameterProperty, "Email");
                    emailButton.Clicked += (s, e) =>
                    {
                        var button = (Button)s;
                        var email = button.CommandParameter as string;
                        var friend = (Friend)button.BindingContext;
                        Email.ComposeAsync("Tervitus!", friend.Message, email);
                    };

                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                photo,
                                new StackLayout { Children = { nameLabel, descriptionLabel, messageEntry }, WidthRequest = 200 },
                                callButton, smsButton, emailButton
                            }
                        }
                    };
                })
            };
            friendsListView.ItemTapped += (s, e) =>
            {
                if (e.Item != null)
                {
                    ((ListView)s).SelectedItem = null;
                }
            };

            var addButton = new Button
            {
                Text = "Lisa uus sõber",
                Command = new Command(AddFriend)
            };

            var greetingsButton = new Button
            {
                Text = "Õnnitlused",
                Command = new Command(SendRandomGreeting)
            };

            Content = new StackLayout
            {
                Children = { addButton, friendsListView, greetingsButton }
            };
        }

        private async void AddFriend()
        {
            string name = await DisplayPromptAsync("Lisa sõber", "Sisesta nimi:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string email = await DisplayPromptAsync("Lisa sõber", "Sisesta e-mail:");
            string phone = await DisplayPromptAsync("Lisa sõber", "Sisesta telefon:");
            string description = await DisplayPromptAsync("Lisa sõber", "Sisesta kirjeldus:");

            var newFriend = new Friend
            {
                Name = name,
                Email = email,
                Phone = phone,
                Description = description
            };

            friends.Add(newFriend);
        }

        private void SendRandomGreeting()
        {
            if (friends.Count == 0) return;
            var friend = friends[random.Next(friends.Count)];
            var message = greetings[random.Next(greetings.Length)];
            Email.ComposeAsync("Õnnitlus!", message, friend.Email);
        }
    }
}
