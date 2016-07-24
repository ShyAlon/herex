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
                Command = new Command(() => Navigation.PushAsync(new AccountsPage()))
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

        private bool ValidEmail(IList<Settings> email)
        {
            if (email == null || email.Count == 0)
            {
                return false;
            }
            var valid = (from s in email
                         where s.Key == Settings.email && !String.IsNullOrEmpty(s.Value)
                         select s.ID).Count();
            return valid > 0;
        }

        protected override void OnAppearing()
        {
            var email = (from s in new Database().GetDBItems<Settings>()
                         where s.Key == Settings.email
                         select s).ToList();

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
                     // End time serves to remove all day events
                     meetings = (from e in res
                                 where e.StartTime > DateTime.Now && e.EndTime > DateTime.Now
                                 orderby e.StartTime ascending
                                 select e).ToDictionary(x => x.InternalID);
                     var db = new Database();
                     foreach (var address in email)
                     {
                         var temp = await db.GetItems<Meeting>(address.Value);
                         var persisted = (temp == null || temp.items == null) ? new Dictionary<long, Meeting>() : CreateDictionary(temp);

                         foreach (var item in persisted.Values)
                         {
                             var f = (from a in email
                                      where a.Value.Equals(item.Originator)
                                      select a).FirstOrDefault();
                             if(f != null) // This is the originator
                             {
                                 meetings[item.InternalID] = item;
                                 meetings[item.InternalID].Tracked = true;
                             }
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

        private Dictionary<long, Meeting> CreateDictionary(CollectionOf<Meeting> temp)
        {
            var res = new Dictionary<long, Meeting>();
            foreach (var item in temp.items)
            {
                res[item.InternalID] = item;
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
