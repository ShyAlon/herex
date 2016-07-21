using System;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace heres.pages
{
    public class PinCode : INotifyPropertyChanged
    {
        public int Number
        {
            get; private set;
        }
        public int Digits
        {
            get
            {
                return Number;
            }

            set
            {
                if (value <= 10000)
                {
                    Number = value;
                }
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SettingsPage : ContentPage
    {
        public string Email {get;set;}
        private PinCode Password { get; set; }

        public SettingsPage()
        {
            Title = nameof(Settings);
            Email = new Database().GetSetting("email");
            var email = new EntryCell { Text = nameof(Email), Label = nameof(Email) };
            email.SetBinding(EntryCell.TextProperty, nameof(Email));
            email.BindingContext = this;
            email.PropertyChanged += Email_PropertyChanged;
            var val = new Database().GetSetting("password");
            Password = new PinCode
            {
                Digits = string.IsNullOrEmpty(val) ? 0 : Int32.Parse(val)
            };
            var password = new EntryCell { Text = nameof(Password), Label = "PIN Code", Keyboard = Keyboard.Numeric };
            password.SetBinding(EntryCell.TextProperty, "Digits");
            password.BindingContext = Password;
            password.PropertyChanged += Password_PropertyChanged;


            var section = new TableSection(nameof(Settings))
            { //TableSection constructor takes title as an optional parameter
                email, password
            };

            var layout = new TableView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Root = new TableRoot
                {
                    section
                },
                Intent = TableIntent.Settings
            };

            Content = layout;
        }

        private void Password_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var db = new Database();
            db.SaveSettings("password", Password.Number.ToString());
        }

        private void Email_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var db = new Database();
            db.SaveSettings("email", Email);
        }
    }
}
