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
        private SwitchCell tracking;
        private Meeting meeting;
        private TableSection section;
        private DataTemplate dataTemplate;

        internal class PersonCell : TextCell
        {
            public PersonCell()
            {
                var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
                deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                deleteAction.Clicked += Delete;
                ContextActions.Add(deleteAction);
            }

            private void Delete(object sender, EventArgs e)
            {
                var p = (Person)BindingContext;
                var db = new Database();
                db.DeleteItem(p);
                EventPage.RefreshList(sender, e);
            }
        }

        internal static event EventHandler RefreshList;

        public EventPage(Meeting _meeting)
        {
            try
            {
                dataTemplate = new DataTemplate(typeof(PersonCell));
                dataTemplate.SetBinding(PersonCell.TextProperty, "Name");
                this.meeting = _meeting;

                ReadParticipants();
                CreateContent();

                RefreshList += (s, e) =>
                {
                    ReadParticipants();
                    CreateContent();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ReadParticipants()
        {
            meeting.Participants = GetParticipants();
            var additional = GetSavedParticipants();
            foreach (var item in additional)
            {
                meeting.Participants.Add(item);
            }
        }

        private void CreateContent()
        {
            BindingContext = meeting;
            Title = meeting.Title;
            tracking = new SwitchCell { On = meeting.Tracked, Text = "Is Tracked" };
            tracking.OnChanged += Tracking_OnChanged;
            var EditEvent = new Button() { Text = "Edit Event" };
            EditEvent.Clicked += OnEditEvent;
            var AddRecipiantEvent = new Button() { Text = "Add Recipiant" };
            AddRecipiantEvent.Clicked += OnAddRecipiant;

            section = new TableSection(meeting.Title)
                { //TableSection constructor takes title as an optional parameter
                    tracking,
                    new TextCell { Detail = meeting.Start.ToString(), Text = "Start"},
                    new TextCell { Detail = meeting.End.ToString(), Text = "End" },
                    new ViewCell { View = EditEvent },
                    new ViewCell { View = AddRecipiantEvent },
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
                                ItemsSource = meeting.Participants,
                                ItemTemplate = dataTemplate,

                            }
                        }
                    }
            };
        }

        private IEnumerable<Person> GetSavedParticipants()
        {
            var db = new Database();
            var persons = db.GetItems<Person>(meeting.InternalID);
            return persons;
        }

        private ObservableCollection<Person> GetParticipants()
        {
            var calendar = DependencyService.Get<ICalendar>();
            var res = calendar.GetParticipants(meeting);
            return new ObservableCollection<Person>(res);
        }

        private void OnEditEvent(object sender, EventArgs e)
        {
            // Open Calendar
            var calendar = DependencyService.Get<ICalendar>();
            calendar.Open(meeting.InternalID);
            App.Resumed += ApplicationResumed;
        }

        async private void OnAddRecipiant(object sender, EventArgs e)
        {
            var page = new SelectContactsPage(meeting);
            await Navigation.PushAsync(page);
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
    }
}
