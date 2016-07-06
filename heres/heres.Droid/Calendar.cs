using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using heres;
using heres.poco;
using Xamarin.Forms;
using Android.Provider;
using static Android.Provider.CalendarContract;
using System.Collections;

[assembly: Dependency(typeof(heres.Droid.Calendar))]

namespace heres.Droid
{
    public class Calendar : ICalendar
    {
        public static String[] EVENT_PROJECTION = {
        Calendars.InterfaceConsts.Id,                           // 0
        Calendars.InterfaceConsts.AccountName,                  // 1
        Calendars.InterfaceConsts.CalendarDisplayName,         // 2
        Calendars.InterfaceConsts.OwnerAccount                  // 3
    };

        // The indices for the projection array above.
        private const int PROJECTION_ID_INDEX = 0;
        private const int PROJECTION_ACCOUNT_NAME_INDEX = 1;
        private const int PROJECTION_DISPLAY_NAME_INDEX = 2;
        private const int PROJECTION_OWNER_ACCOUNT_INDEX = 3;

        public IEnumerable<Meeting> GetEvents()
        {
            // List Calendars
            var calendarsUri = CalendarContract.Calendars.ContentUri;

            string[] calendarsProjection = {
               CalendarContract.Calendars.InterfaceConsts.Id,
               CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
               CalendarContract.Calendars.InterfaceConsts.AccountName
            };

            var result = new List<Meeting>();

            var cursor = ((Activity)Forms.Context).ManagedQuery(calendarsUri, calendarsProjection, null, null, null);
            while (cursor.MoveToNext())
            {
                long calId = cursor.GetLong(PROJECTION_ID_INDEX);
                String displayName = cursor.GetString(PROJECTION_DISPLAY_NAME_INDEX);
                String accountName = cursor.GetString(PROJECTION_ACCOUNT_NAME_INDEX);

                var eventsUri = CalendarContract.Events.ContentUri;

                string[] eventsProjection = {
                    CalendarContract.Events.InterfaceConsts.Id,
                    CalendarContract.Events.InterfaceConsts.Title,
                    CalendarContract.Events.InterfaceConsts.Dtstart,
                    CalendarContract.Events.InterfaceConsts.Dtend
                 };

                var eventcursor = ((Activity)Forms.Context).ManagedQuery(eventsUri, eventsProjection,
                 string.Format("calendar_id={0}", calId), null, "dtstart ASC");

                while (eventcursor.MoveToNext())
                {
                    var m = new Meeting()
                    {
                        CalendarId = calId,
                        Id = eventcursor.GetLong(0),
                        Title = eventcursor.GetString(1),
                        StartTime = GetTime(eventcursor.GetLong(2)),
                        EndTime = GetTime(eventcursor.GetLong(3))
                    };
                    result.Add(m);
                }
            }

            return result;
        }

        private DateTime GetTime(long val)
        {
            var res = new DateTime(val * 10000);
            res = res.AddYears(1969);
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            res = res.Add(offset);
            
            return res;
        }
    }
}