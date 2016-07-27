using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
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

            }

            protected override void OnBindingContextChanged()
            {
                base.OnBindingContextChanged();
                var p = (Person)BindingContext;
                if (!p.Organic)
                {
                    var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
                    deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                    deleteAction.Clicked += Delete;
                    ContextActions.Add(deleteAction);
                }
            }

            private async void Delete(object sender, EventArgs e)
            {
                var p = (Person)BindingContext;
                var db = new Database();
                await db.DeleteItem(p, db.PrimaryEmail);
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

                RefreshListHandler(this, new EventArgs());
                RefreshList += RefreshListHandler;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void RefreshListHandler(object sender, EventArgs e)
        {
            ReadParticipants();
            CreateContent();
        }

        private async void ReadParticipants()
        {
            try
            {
                meeting.Participants = GetParticipants();
                var additional = await GetSavedParticipants();
                if (additional != null)
                {
                    foreach (var item in additional)
                    {
                        meeting.Participants.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
        }

        private void CreateContent()
        {
            BindingContext = meeting;
            Title = meeting.Title;
            tracking = new SwitchCell { On = meeting.Tracked, Text = "Is Tracked" };
            tracking.OnChanged += TrackingChangedAsync;
            var EditEvent = new Button() { Text = "Edit Event" };
            EditEvent.Clicked += OnEditEvent;
            var AddRecipiantEvent = new Button() { Text = "Add Recipiant" };
            AddRecipiantEvent.Clicked += OnAddRecipiant;

            section = new TableSection(meeting.Title)
            { //TableSection constructor takes title as an optional parameter
                tracking,
                new TextCell { Detail = meeting.StartTime.ToString(), Text = "Start"},
                new TextCell { Detail = meeting.EndTime.ToString(), Text = "End" },
                new ViewCell { View = EditEvent },
                new ViewCell { View = AddRecipiantEvent },
            };

            var listView = new ListView
            {
                Header = "Participants",
                ItemsSource = meeting.Participants,
                ItemTemplate = dataTemplate,
            };
            listView.ItemTapped += ParticipantTapped;

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
                           Content = listView
                        }
                    }
            };
        }

        async private void ParticipantTapped(object sender, ItemTappedEventArgs e)
        {
            var detailPage = new PersonPage((Person)e.Item);
            await Navigation.PushAsync(detailPage);
        }

        private async Task<IEnumerable<Person>> GetSavedParticipants()
        {
            try
            {
                var db = new Database();
                var addresses = db.GetEmailAddresses();
                var result = new List<Person>();
                var persons = await db.GetItems<Person>(meeting.ID, db.PrimaryEmail);
                if (persons != null && persons.items != null)
                {
                    foreach (var item in persons.items)
                    {
                        item.Organic = false;
                        result.Add(item);
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private ObservableCollection<Person> GetParticipants()
        {
            var calendar = DependencyService.Get<ICalendar>();
            var res = calendar.GetParticipants(meeting);
            foreach (var item in res)
            {
                item.ID = item.ContactID + 100000;
                item.Organic = true;
            }
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
            if (m != null)
            {
                meeting.StartTime = m.StartTime;
                meeting.EndTime = m.EndTime;
                meeting.Title = m.Title;
            }
            Title = meeting.Title;
            section.Title = Title;
        }

        protected override void OnDisappearing()
        {
            RefreshList -= RefreshListHandler;
            var db = new Database();
            base.OnDisappearing();
            if (meeting.Tracked)
            {
                db.SaveItem(meeting, db.PrimaryEmail).ContinueWith((res) => meeting.ID = res.Result);
            }
        }

        private async void TrackingChangedAsync(object sender, ToggledEventArgs e)
        {
            var db = new Database();
            var primaryEmail = db.PrimaryEmail;
            if (e.Value)
            {
                meeting.Originator = primaryEmail;
                meeting.ID = await db.SaveItem(meeting, db.PrimaryEmail);
                var participant = new Person
                {
                    ParentID = meeting.ID,
                    Name = db.GetSetting(Settings.name),
                    IsOrganizer = 1,
                    Email = primaryEmail,
                };
                participant.ID = await db.SaveItem(participant, db.PrimaryEmail);
            }
            else
            {
                await db.DeleteItem(meeting, db.PrimaryEmail);
                meeting.ID = 0;
                meeting.Participants = null;
                await Navigation.PopAsync();
            }
            RefreshList?.Invoke(sender, e);
        }
    }
}
