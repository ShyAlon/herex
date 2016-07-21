using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
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

            this.ToolbarItems.Add(new ToolbarItem
            {
                Icon = "settings.png",
                Command = new Command(() => Navigation.PushAsync(new SettingsPage()))
            });
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
            var email = new Database().GetSetting("email");
            var val = new Database().GetSetting("password");

            if (string.IsNullOrEmpty(email))
            {
                var page = new SettingsPage();
                Navigation.PushAsync(page);
            }
            var start = new Task(async () =>
             {
                 var dialer = DependencyService.Get<ICalendar>();
                 var res = dialer.GetEvents();
                 // End time serves to remove all day events
                 meetings = (from e in res
                             where e.StartTime > DateTime.Now && e.EndTime > DateTime.Now
                             orderby e.StartTime ascending
                             select e).ToDictionary(x => x.InternalID);
                 var db = new Database();
                 var temp = await db.GetItems<Meeting>(email);
                 var persisted = temp.items.ToDictionary(x => x.InternalID);

                 foreach (var item in meetings.Values)
                 {
                     item.Tracked = (persisted.ContainsKey(item.InternalID));
                     if (item.Tracked)
                     {
                         item.ID = persisted[item.InternalID].ID;
                     }
                 }

                 foreach (var item in persisted.Values)
                 {
                     item.Tracked = true;
                     item.InternalID = 0;
                     meetings[item.ID] = item;
                 }

                 Device.BeginInvokeOnMainThread(() =>
                 {
                     GenerateList();
                 });
             });
            {
                start.ContinueWith((t) => start.Dispose());
                start.Start();
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
