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

            var layout = new StackLayout()
            {
                Children =
                {
                    new Label
                    {
                        Text = "Loading Events"
                    }
                }, VerticalOptions = LayoutOptions.Center

            };
            Content = new ScrollView
            {
                Content = layout
            };
            

            Task.Run(() =>
            {
                Thread.Sleep(300);
                var dialer = DependencyService.Get<ICalendar>();
                var res = dialer.GetEvents();
                var relevant = (from e in res
                               where e.StartTime > DateTime.Now
                               orderby e.StartTime ascending
                               select e).ToList();

                Device.BeginInvokeOnMainThread(() =>
                {
                    Content.BatchBegin();
                    list = new ListView
                    {
                        ItemsSource = res,
                        ItemTemplate = dataTemplate
                    };
                    ((ScrollView)Content).Content = list;
                    Content.BatchCommit();

                    list.ItemSelected += EventSelected;
                    
                });
            });
        }

        async private void EventSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //if (e.SelectedItem == null)
            //{
            //    return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            //}
            //DisplayAlert("Item Selected", e.SelectedItem.ToString(), "Ok");
            //((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual se

            await Navigation.PushAsync(new heres.pages.EventPage());
        }

        void OnButtonClicked(object sender, EventArgs args)
        { // Add Label to scrollable StackLayout.
            System.Console.WriteLine(sender.ToString());
        }
    }
}
