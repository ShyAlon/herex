using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using heres.components;

namespace heres.pages
{
    public class EventsPage : ContentPage
    {
        private ListView list;

        public EventsPage()
        {
            DataTemplate dataTemplate = new DataTemplate(typeof(MeetingCell));
            dataTemplate.SetBinding(TextCell.TextProperty, "Title");

            Title = "Events";

            var layout = new StackLayout()
            {
                Children =
                {
                    new ActivityIndicator
                    {
                        IsRunning = true
                    }
                }, VerticalOptions = LayoutOptions.Center

            };
            Content = layout;
            

            new Thread(() =>
            {
                var dialer = DependencyService.Get<ICalendar>();
                var res = dialer.GetEvents();
                var relevant = (from e in res
                               where e.StartTime > DateTime.Now
                               orderby e.StartTime ascending
                               select e).ToList();

                Device.BeginInvokeOnMainThread(() =>
                {
                    list = new ListView
                    {
                        ItemsSource = relevant,
                        ItemTemplate = dataTemplate
                    };
                    Content = new ScrollView
                    {
                        Content = list
                    };
                    ((ScrollView)Content).Content = list;
                    list.ItemSelected += EventSelected;
                    
                });
            }).Start();
        }

        async private void EventSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //if (e.SelectedItem == null)
            //{
            //    return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            //}
            //DisplayAlert("Item Selected", e.SelectedItem.ToString(), "Ok");
            //((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual se
            var page = new EventPage(e.SelectedItem as Meeting);
            page.BindingContext = e.SelectedItem;
            await Navigation.PushAsync(page);
        }
    }
}
