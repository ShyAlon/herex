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
        private DatePicker startDatePicker;
        private DatePicker endDatePicker;
        private TimePicker startTimePicker;
        private TimePicker endTimePicker;
        private SwitchCell tracking;
        private Meeting meeting;

        public EventPage(Meeting _meeting)
        {
            try
            {
                this.meeting = _meeting;
                BindingContext = meeting;
                Title = meeting.Title;
                startDatePicker = new DatePicker();
                endDatePicker = new DatePicker();
                startTimePicker = new TimePicker();
                endTimePicker = new TimePicker();
                tracking = new SwitchCell { On = meeting.Tracked, Text = "Is Tracked" };
                tracking.OnChanged += Tracking_OnChanged;

                //title.SetBinding(Label.TextProperty, "Title");
                startDatePicker.SetBinding(DatePicker.DateProperty, "StartDate");
                endDatePicker.SetBinding(DatePicker.DateProperty, "EndDate");
                startTimePicker.SetBinding(TimePicker.TimeProperty, "StartTime");
                endTimePicker.SetBinding(TimePicker.TimeProperty, "EndTime");

                var section = new TableSection(meeting.Title) { //TableSection constructor takes title as an optional parameter
                tracking,
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private void Tracking_OnChanged(object sender, ToggledEventArgs e)
        {
            var db = new Database();
            if (e.Value)
            {
                meeting.ID = db.SaveItem(meeting);
            }
            else
            {
                db.DeleteItem(meeting.ID);
            }
        }

        private static StackLayout Flach(string title, View view)
        {
            var layout = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(20, 0, 10, 0) };
            view.VerticalOptions = LayoutOptions.Center;
            view.HorizontalOptions = LayoutOptions.EndAndExpand;
            layout.Children.Add(new Label()
            {
                Text = title,
                TextColor = Color.FromHex("#f35e20"),
                VerticalOptions = LayoutOptions.Center
            });
            layout.Children.Add(view);
            return layout;
        }

        private static StackLayout Flach(string title, View view1, View view2)
        {
            var layout = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(20, 0, 10, 0) };
            view1.VerticalOptions = LayoutOptions.Center;
            view1.HorizontalOptions = LayoutOptions.CenterAndExpand;
            view2.VerticalOptions = LayoutOptions.Center;
            view2.HorizontalOptions = LayoutOptions.EndAndExpand;
            layout.Children.Add(new Label()
            {
                Text = title,
                TextColor = Color.FromHex("#f35e20"),
                VerticalOptions = LayoutOptions.Center
            });
            layout.Children.Add(view1);
            layout.Children.Add(view2);
            return layout;
        }
    }
}
