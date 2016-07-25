using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace heres.pages
{
    public class SelectContactsPage : ContentPage
    {
        private readonly Meeting meeting;

        public SelectContactsPage(Meeting _meeting)
        {
            try
            {
                meeting = _meeting;
                var calendar = DependencyService.Get<ICalendar>();
                var temp = calendar.GetParticipantNames().OrderBy(s => s);
                var lookup = meeting.Participants.ToLookup(p => p.Name);
                var res = new List<string>();
                foreach (var item in temp)
                {
                    if(!lookup.Contains(item))
                    {
                        res.Add(item);
                    }
                }
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

        private async void ContactSelected(object sender, ItemTappedEventArgs e)
        {
            try
            {
                if (meeting.Participants == null)
                {
                    meeting.Participants = new List<Person>();
                }
                var calendar = DependencyService.Get<ICalendar>();
                var p = calendar.GetContact(e.Item.ToString());
                p.ParentID = meeting.ID;
                meeting.Participants.Add(p);
                var db = new Database();
                var id = await db.SaveItem(p);
                await Navigation.PopAsync();
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
