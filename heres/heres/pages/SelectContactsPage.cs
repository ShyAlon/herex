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
    public class SelectContactsPage : ContentPage
    {
        private readonly SwitchCell tracking;
        private readonly Meeting meeting;
        private readonly TableSection section;
        private ObservableCollection<Person> participants;
        private DataTemplate dataTemplate;

        public SelectContactsPage(Meeting _meeting)
        {
            try
            {
                meeting = _meeting;
                var calendar = DependencyService.Get<ICalendar>();
                var res = calendar.GetParticipantNames();
                var lv = new ListView
                {
                    Header = "Contacts",
                    ItemsSource = res,
                };
                lv.ItemTapped += ContactSelected;
                Content = new ScrollView
                {
                    Content = lv
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ContactSelected(object sender, ItemTappedEventArgs e)
        {
            try
            {
                if (meeting.Participants == null)
                {
                    meeting.Participants = new List<Person>();
                }
                var calendar = DependencyService.Get<ICalendar>();
                var p = calendar.GetContact(e.Item.ToString());
                meeting.Participants.Add(p);
                p.ParentID = meeting.InternalID;
                var db = new Database();
                db.SaveItem(p);
                Navigation.PopAsync();
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
        }
    }
}
