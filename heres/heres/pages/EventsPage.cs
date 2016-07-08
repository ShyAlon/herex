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
        private Dictionary<long, Meeting> meetings;
        private PageTypeGroup tracked;
        private PageTypeGroup unTracked;
        private DataTemplate dataTemplate;

        public EventsPage()
        {
            dataTemplate = new DataTemplate(typeof(MeetingCell));
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
                },
                VerticalOptions = LayoutOptions.Center

            };
            Content = layout;


            var start = new Task(() =>
            {
                var dialer = DependencyService.Get<ICalendar>();
                var res = dialer.GetEvents();
                // End time serves to remove all day events
                meetings = (from e in res
                            where e.Start > DateTime.Now && e.End > DateTime.Now
                            orderby e.Start ascending
                            select e).ToDictionary(x => x.InternalID);
                var db = new Database();
                var persisted = db.GetItems().ToDictionary(x => x.InternalID);

                foreach (var item in meetings.Values)
                {
                    item.Tracked = (persisted.ContainsKey(item.InternalID));
                    if(item.Tracked)
                    {
                        item.ID = persisted[item.InternalID].ID;
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    GenerateList();
                });
            });

            start.Start();
        }

        private void GenerateList()
        {
            tracked = new PageTypeGroup("Tracked", "T", from t in meetings.Values where t.Tracked select t);
            unTracked = new PageTypeGroup("Untracked", "U", from t in meetings.Values where !t.Tracked select t);

            var all = new List<PageTypeGroup>
                {
                    tracked, unTracked
                };

            list = new ListView
            {
                ItemsSource = all,
                ItemTemplate = dataTemplate,
                IsGroupingEnabled = true,
                GroupDisplayBinding = new Binding("Title"),
                GroupShortNameBinding = new Binding("ShortName"),
            };
            Content = new ScrollView
            {
                Content = list
            };
            ((ScrollView)Content).Content = list;
            list.ItemTapped += List_ItemTapped;
        }

        async private void List_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var page = new EventPage(e.Item as Meeting);
            await Navigation.PushAsync(page);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(list != null)
            {
                list.SelectedItem = null;
            };

            if(meetings != null && meetings.Count > 0)
            {
                GenerateList();
            }
        }

        public class PageTypeGroup : List<Meeting>
        {
            public string Title { get; set; }
            public string ShortName { get; set; } //will be used for jump lists
            public string Subtitle { get; set; }
            public PageTypeGroup(string title, string shortName, IEnumerable<Meeting> items)
            {
                Title = title;
                ShortName = shortName;
                AddRange(items);
            }

            public static IList<PageTypeGroup> All { private set; get; }
        }
    }
}
