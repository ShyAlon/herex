using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using heres.components;
using System.Reflection;

namespace heres.pages
{
    public class EventPage : ContentPage
    {
        private Label title;
        private DatePicker startDatePicker;
        private DatePicker endDatePicker;
        private TimePicker startTimePicker;
        private TimePicker endTimePicker;

        public EventPage(Meeting meeting)
        {
            Title = meeting.Title;
            startDatePicker = new DatePicker();
            endDatePicker = new DatePicker();
            startTimePicker = new TimePicker();
            endTimePicker = new TimePicker();

            //title.SetBinding(Label.TextProperty, "Title");
            startDatePicker.SetBinding(DatePicker.DateProperty, "StartTime");
            endDatePicker.SetBinding(DatePicker.DateProperty, "EndTime");
            startTimePicker.SetBinding(TimePicker.TimeProperty, "StartTime");
            endTimePicker.SetBinding(TimePicker.TimeProperty, "EndTime");

            var section = new TableSection(meeting.Title) { //TableSection constructor takes title as an optional parameter
                new SwitchCell {Text = "Follow Up"},
                new ViewCell {View = Flach("Start:", startDatePicker, startTimePicker) },
                new ViewCell {View = Flach("End:", endDatePicker, endTimePicker) },
            };
            Content = new TableView
            {
                Root = new TableRoot
                {
                    section
                },
                Intent = TableIntent.Settings
            };
        }

        private StackLayout Flach(string title, View view)
        {
            var layout = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(20, 0, 10, 0) };
            view.VerticalOptions = LayoutOptions.Center;
            view.HorizontalOptions = LayoutOptions.EndAndExpand;
            layout.Children.Add(new Label()
            {
                Text = "Start Date",
                TextColor = Color.FromHex("#f35e20"),
                VerticalOptions = LayoutOptions.Center
            });
            layout.Children.Add(view);
            return layout;
        }

        private StackLayout Flach(string title, View view1, View view2)
        {
            var layout = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(20, 0, 10, 0) };
            view1.VerticalOptions = LayoutOptions.Center;
            view1.HorizontalOptions = LayoutOptions.CenterAndExpand;
            view2.VerticalOptions = LayoutOptions.Center;
            view2.HorizontalOptions = LayoutOptions.EndAndExpand;
            layout.Children.Add(new Label()
            {
                Text = "Start Date",
                TextColor = Color.FromHex("#f35e20"),
                VerticalOptions = LayoutOptions.Center
            });
            layout.Children.Add(view1);
            layout.Children.Add(view2);
            return layout;
        }
    }
}
