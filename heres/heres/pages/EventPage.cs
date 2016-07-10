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
using System.Collections.ObjectModel;

namespace heres.pages
{
    public class EventPage : ContentPage
    {
        private readonly SwitchCell tracking;
        private readonly Meeting meeting;
        private readonly TableSection section;
        private ObservableCollection<Person> participants;
        private DataTemplate dataTemplate;

        internal class PersonCell : TextCell
        {
            public PersonCell()
            {
                var moreAction = new MenuItem { Text = "More" };
                moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                moreAction.Clicked += async (sender, e) => {
                    var mi = ((MenuItem)sender);
                    Console.WriteLine("More Context Action clicked: " + mi.CommandParameter);
                };

                var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
                deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                deleteAction.Clicked += async (sender, e) => {
                    var mi = ((MenuItem)sender);
                    Console.WriteLine("Delete Context Action clicked: " + mi.CommandParameter);
                };
                // add to the ViewCell's ContextActions property
                ContextActions.Add(moreAction);
                ContextActions.Add(deleteAction);
            }
        }

        public EventPage(Meeting _meeting)
        {
            try
            {
                dataTemplate = new DataTemplate(typeof(PersonCell));
                dataTemplate.SetBinding(PersonCell.TextProperty, "Name");
                this.meeting = _meeting;
                BindingContext = meeting;
                Title = meeting.Title;
                tracking = new SwitchCell { On = meeting.Tracked, Text = "Is Tracked" };
                tracking.OnChanged += Tracking_OnChanged;
                var EditEvent = new Button() { Text = "Edit Event" };
                EditEvent.Clicked += OnEditEvent;
                var AddRecipiantEvent = new Button() { Text = "Add Recipiant" };
                AddRecipiantEvent.Clicked += OnAddRecipiant;


                participants = GetParticipants();

                section = new TableSection(meeting.Title)
                { //TableSection constructor takes title as an optional parameter
                    tracking,
                    new TextCell { Detail = meeting.Start.ToString(), Text = "Start"},
                    new TextCell { Detail = meeting.End.ToString(), Text = "End" },
                    new ViewCell {View = EditEvent },
                    new ViewCell {View = AddRecipiantEvent },
                };

                Content = new StackLayout
                {
                    Children =
                    {
                        new TableView
                        {
                            VerticalOptions = LayoutOptions.Start,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Root = new TableRoot
                            {
                                section
                            }, Intent = TableIntent.Settings
                        },

                        new ScrollView
                        {
                           Content = new ListView
                            {
                               Header = "Participants",
                                ItemsSource = participants,
                                ItemTemplate = dataTemplate,
                               
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private ObservableCollection<Person> GetParticipants()
        {
            var calendar = DependencyService.Get<ICalendar>();
            var res = calendar.GetParticipants(meeting);
            return new ObservableCollection<Person>(res);
            // End time serves to remove all day events
            //meetings = (from e in res
            //            where e.Start > DateTime.Now && e.End > DateTime.Now
            //            orderby e.Start ascending
            //            select e).ToDictionary(x => x.InternalID);
            //var db = new Database();
            //var persisted = db.GetItems<Meeting>().ToDictionary(x => x.InternalID);
        }

        private void OnEditEvent(object sender, EventArgs e)
        {
            // Open Calendar
            var calendar = DependencyService.Get<ICalendar>();
            calendar.Open(meeting.InternalID);
            App.Resumed += ApplicationResumed;
        }

        private void OnAddRecipiant(object sender, EventArgs e)
        {
            participants.Add(new Person { Name = "Mayah" });
        }

        private void ApplicationResumed(object sender, EventArgs e)
        {
            App.Resumed -= ApplicationResumed;
            OnAppearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var calendar = DependencyService.Get<ICalendar>();
            var m = calendar.GetEvent(meeting);
            meeting.Start = m.Start;
            meeting.End = m.End;
            meeting.Title = m.Title;
            Title = meeting.Title;
            section.Title = Title;
        }

        protected override void OnDisappearing()
        {
            var db = new Database();
            base.OnDisappearing();
            if (meeting.Tracked)
            {
                meeting.ID = db.SaveItem(meeting);
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
                db.DeleteItem(meeting);
            }
            meeting.Tracked = e.Value;
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
