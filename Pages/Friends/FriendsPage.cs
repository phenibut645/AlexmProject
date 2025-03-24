using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using alexm_app.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Storage;

namespace alexm_app.Pages.Friends
{
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

        private const string FileName = "friends.xml";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        private ListView friendsListView;

        public FriendsPage()
        {
            friends = LoadFriends();

            friendsListView = new ListView
            {
                ItemsSource = friends,
                ItemTemplate = new DataTemplate(() =>
                {
                    Image photo = new Image { HeightRequest = 50, WidthRequest = 100, HorizontalOptions = LayoutOptions.Start };
                    photo.SetBinding(Image.SourceProperty, "Photo");

                    Label nameLabel = new Label { FontSize = 20, HorizontalTextAlignment = TextAlignment.Start};
                    nameLabel.SetBinding(Label.TextProperty, "Name");

                    Label descriptionLabel = new Label { FontSize = 14, HorizontalTextAlignment = TextAlignment.Start };
                    descriptionLabel.SetBinding(Label.TextProperty, "Description");

                    Entry messageEntry = new Entry { Placeholder = "Sisesta sõnum" };
                    messageEntry.SetBinding(Entry.TextProperty, "Message");

                    Button callButton = new Button { Text = "📞", WidthRequest = 60, HorizontalOptions = LayoutOptions.End , Padding = 5, HeightRequest=30};
                    callButton.SetBinding(Button.CommandParameterProperty, "Phone");
                    callButton.Clicked += (s, e) =>
                    {
                        var button = (Button)s;
                        var phone = button.CommandParameter as string;
                        if (!string.IsNullOrEmpty(phone))
                            PhoneDialer.Open(phone);
                    };

                    Button smsButton = new Button { Text = "✉️", WidthRequest = 60, HorizontalOptions = LayoutOptions.End, Padding = 5, HeightRequest = 30 };
                    smsButton.SetBinding(Button.CommandParameterProperty, "Phone");
                    smsButton.Clicked += async (s, e) =>
                    {
                        Button button = (Button)s;
                        string phone = button.CommandParameter as string;
                        Friend friend = (Friend)button.BindingContext;
                        if (!string.IsNullOrEmpty(phone))
                        {
                            await Sms.ComposeAsync(new SmsMessage(friend.Message, phone));
                        }
                    };

                    Button emailButton = new Button { Text = "📧", WidthRequest = 60, HorizontalOptions = LayoutOptions.End, Padding = 5, HeightRequest = 30 };
                    emailButton.SetBinding(Button.CommandParameterProperty, "Email");
                    emailButton.Clicked += async (s, e) =>
                    {
                        Button button = (Button)s;
                        string email = button.CommandParameter as string;
                        Friend friend = (Friend)button.BindingContext;
                        if (!string.IsNullOrEmpty(email))
                        {
                            await Email.ComposeAsync("Tervitus!", friend.Message, email);
                        }
                    };

                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,

                            Children =
                            {
                                photo,
                                new StackLayout { Children = { nameLabel, descriptionLabel, messageEntry }, WidthRequest = 100},
                                callButton, smsButton, emailButton
                            },
                            MinimumHeightRequest = 200,
                            
                        }
                    };
                }),
                RowHeight = 100

            };
            friendsListView.ItemTapped += (s, e) =>
            {
                if (e.Item != null)
                {
                    ((ListView)s).SelectedItem = null;
                }
            };

            Button addButton = new Button
            {
                Text = "Lisa uus sõber",
                Command = new Command(async () => await AddFriend())
            };

            Button greetingsButton = new Button
            {
                Text = "Õnnitlused",
                Command = new Command(SendRandomGreeting)
            };

            Content = new StackLayout
            {
                Children = { addButton, friendsListView, greetingsButton }
            };
        }

        private async Task AddFriend()
        {
            string name = await DisplayPromptAsync("Lisa sõber", "Sisesta nimi:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string email = await DisplayPromptAsync("Lisa sõber", "Sisesta e-mail:", keyboard: Keyboard.Email);
            string phone = await DisplayPromptAsync("Lisa sõber", "Sisesta telefon:", keyboard: Keyboard.Telephone);
            string description = await DisplayPromptAsync("Lisa sõber", "Sisesta kirjeldus:");

            string photoPath = null;

            try
            {
                FileResult? file = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Vali pilt",
                    FileTypes = FilePickerFileType.Images
                });

                if (file != null)
                {
                    string destinationFolder = Path.Combine(FileSystem.AppDataDirectory, "Images");
                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }

                    string destinationPath = Path.Combine(destinationFolder, file.FileName);
                    using (var stream = await file.OpenReadAsync())
                    using (var newStream = File.Create(destinationPath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    photoPath = destinationPath;
                }

                Friend newFriend = new Friend
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Photo = photoPath,
                    Description = description
                };

                friends.Add(newFriend);
                SaveFriends();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private async void SendRandomGreeting()
        {
            if (friends.Count == 0) return;
            Friend friend = friends[random.Next(friends.Count)];
            string message = greetings[random.Next(greetings.Length)];
            if (!string.IsNullOrEmpty(friend.Email))
            {
                await Email.ComposeAsync("Õnnitlus!", message, friend.Email);
            }
        }

        private void SaveFriends()
        {
            try
            {
                using (var writer = new StreamWriter(FilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Friend>));
                    serializer.Serialize(writer, friends);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("XML Error: " + e.Message);
            }
        }

        private ObservableCollection<Friend> LoadFriends()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    using (var reader = new StreamReader(FilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Friend>));
                        return (ObservableCollection<Friend>)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("XML error: " + e.Message);
            }
            return new ObservableCollection<Friend>();
        }
    }
}
