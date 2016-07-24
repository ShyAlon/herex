using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

namespace heres.pages
{
    public class AccountsPage : ContentPage
    {
        private Entry newEmail;
        private DataTemplate dataTemplate;
        private IList<Settings> settings;

        public PinCode Password { get; private set; }
        public string Name { get; private set; }

        internal static event EventHandler RefreshList;

        internal class EmailCell : TextCell
        {
            public EmailCell()
            {

            }

            protected override void OnBindingContextChanged()
            {
                base.OnBindingContextChanged();
                var p = (Settings)BindingContext;
                Text = p.Value;
                var deleteAction = new MenuItem { Text = @"Delete", IsDestructive = true }; // red background
                deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                deleteAction.Clicked += Delete;
                ContextActions.Add(deleteAction);
            }

            private void Delete(object sender, EventArgs e)
            {
                var p = (Settings)BindingContext;
                var db = new Database();
                var id = db.DeleteDBItem(p);
                AccountsPage.RefreshList(sender, e);
            }
        }


        public AccountsPage()
        {
            dataTemplate = new DataTemplate(typeof(EmailCell));
            dataTemplate.SetBinding(EmailCell.TextProperty, "Name");
            // NavigationPage.SetHasBackButton(this, !Warn);
            try
            {
                AccountsPage_RefreshList(this, null);
                RefreshList += AccountsPage_RefreshList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void AccountsPage_RefreshList(object sender, EventArgs e)
        {
            var db = new Database();
            settings = (from s in db.GetDBItems<Settings>()
                       where s.Key == Settings.email
                       select s).ToList();
            CreateContent();
            OnPropertyChanged(nameof(Warn));
        }

        private void CreateContent()
        {
            var db = new Database();
            var AddEmail = new Button
            {
                Text = "Add Email Address"
            };
            AddEmail.Clicked += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(newEmail.Text))
                {
                    var r = new Settings
                    {
                        Key = Settings.email,
                        Value = newEmail.Text
                    };

                    db.SaveDBItem(r);
                    settings.Add(r);
                    newEmail.Text = string.Empty;
                    OnPropertyChanged(nameof(Warn));
                }
            };

            newEmail = new Entry() { };
            var val = db.GetSetting(Settings.pin);
            int _limit = 12;    //Enter text limit
            var pin = new Entry()
            {
                Text = val, IsPassword = true
            };
            pin.TextChanged += (sender, args) =>
            {
                string _text = pin.Text;      //Get Current Text
                if (_text.Length > _limit)       //If it is more than your character restriction
                {
                    _text = _text.Remove(_text.Length - 1);  // Remove Last character
                    pin.Text = _text;        //Set the Old value
                }
                db.SaveSettings(Settings.pin, pin.Text);
            };

            val = db.GetSetting(Settings.name);
            var name = new Entry
            {
                Text = val, 
            };

            name.TextChanged += (sender, args) =>
            {
                db.SaveSettings(Settings.name, name.Text);
            };


            AddEmail.SetBinding(Button.IsEnabledProperty, new Binding(@"HasEmail"));
            var listView = new ListView
            {
                Header = "Email Addreses",
                ItemsSource = settings,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ItemTemplate = dataTemplate,
            };

            var warn = new Label
            {
                Text = "Must have at least one email account to continue",
                TextColor = Color.Red,
                BindingContext = this
            };
            warn.SetBinding(Label.IsVisibleProperty, nameof(Warn));

            // listView.ItemTapped += ParticipantTapped;

            Content = new StackLayout
            {
                Children =
                    {
                        new StackLayout { Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                warn,
                                new Label {Text = "Password:"},
                                pin,
                                new Label {Text = "Name:",},
                                name,
                                new Label {Text = ""},
                                new Label {Text = "New Email:"},
                                newEmail,
                            },
                        },
                        AddEmail,
                        new ScrollView
                        {
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Content = listView,
                        }
                    }
            };
        }

        public bool HasEmail() => !string.IsNullOrWhiteSpace(newEmail.Text);

        public bool Warn
        {
            get
            {
                return settings == null || settings.Count == 0;
            }
        }
    }
}
