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
        private Dictionary<string, Meeting> meetings;
        private PageTypeGroup tracked;
        private PageTypeGroup external;
        private PageTypeGroup unTracked;
        private DataTemplate dataTemplate;
        private HashSet<string> email;

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
                Command = new Command(() => Navigation.PushAsync(new AccountsPage()))
            });
        }

        private void GenerateList()
        {
            tracked = new PageTypeGroup("Tracked", "T", from t in meetings.Values where t.Tracked && email.Contains(t.Originator) select t);
            external = new PageTypeGroup("External", "E", from t in meetings.Values where t.Tracked && !email.Contains(t.Originator) select t);
            unTracked = new PageTypeGroup("Untracked", "U", from t in meetings.Values where !t.Tracked select t);

            var all = new List<PageTypeGroup>
                {
                    tracked, external, unTracked
                };
                
            list = new ListView
            {
                ItemsSource = all,
                ItemTemplate = dataTemplate,
                IsGroupingEnabled = true,
                GroupDisplayBinding = new Binding("Title"),
                GroupShortNameBinding = new Binding("ShortName"),
                // GroupHeaderTemplate = new DataTemplate(typeof(HeaderCell))
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

        private static bool ValidEmail(HashSet<string> email)
        {
            if (email == null || email.Count == 0)
            {
                return false;
            }
            var valid = (from s in email
                         where !String.IsNullOrEmpty(s)
                         select s).Count();
            return valid > 0;
        }

        protected override void OnAppearing()
        {
            email = new HashSet<string>(new Database().GetEmailAddresses());

            if (!ValidEmail(email))
            {
                Navigation.PushAsync(new AccountsPage());
            }
            Task start = null;
            start = new Task(async () =>
             {
                 try
                 {
                     var dialer = DependencyService.Get<ICalendar>();
                     var res = dialer.GetEvents();
                     res = res ?? new List<Meeting>();
                     // End time serves to remove all day events
                     var db = new Database();
                     foreach (var item in res)
                     {
                         item.Originator = item.Originator ?? db.PrimaryEmail;
                         item.ID = 0;
                     }
                     meetings = (from e in res
                                 where e.StartTime > DateTime.Now && e.EndTime > DateTime.Now
                                 orderby e.StartTime ascending
                                 select e).ToDictionary(x => $"{x.InternalID}{x.Originator}");
                     
                     
                     foreach (var address in email)
                     {
                         var temp = await db.GetItems<Meeting>(0, address);
                         var persisted = (temp == null || temp.items == null) ? new Dictionary<string, Meeting>() : CreateDictionary(temp);
                         var local = meetings.Values.ToList();

                         foreach (var item in persisted.Values)
                         {
                             // override with persisted data
                             meetings[$"{item.InternalID}{item.Originator}"] = item;
                         }
                     }
                     Device.BeginInvokeOnMainThread(() =>
                     {
                         GenerateList();
                         start.Dispose();
                     });
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine(ex);
                     throw;
                 }

             });
            {
                start.Start();
            }
        }

        private Dictionary<string, Meeting> CreateDictionary(CollectionOf<Meeting> temp)
        {
            var res = new Dictionary<string, Meeting>();
            foreach (var item in temp.items)
            {
                res[$"{item.InternalID}{item.Originator}"] = item;
            }
            return res;
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
