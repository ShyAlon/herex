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

        public static IEnumerable<Meeting> Meetings { get; set; }

        public IEnumerable<Meeting> GetEvents(Object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;
            if (Meetings != null)
            {
                return Meetings;
            }
            // List Calendars
            var calendarsUri = CalendarContract.Calendars.ContentUri;

            string[] calendarsProjection = {
               CalendarContract.Calendars.InterfaceConsts.Id,
               CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
               CalendarContract.Calendars.InterfaceConsts.AccountName
            };

            var result = new List<Meeting>();

            var cursor = con.ManagedQuery(calendarsUri, calendarsProjection, null, null, null);
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

                var eventcursor = con.ManagedQuery(eventsUri, eventsProjection, $"calendar_id={calId}", null, "dtstart ASC");

                while (eventcursor.MoveToNext())
                {
                    var m = new Meeting()
                    {
                        CalendarId = calId,
                        InternalID = eventcursor.GetLong(0),
                        Title = eventcursor.GetString(1),
                        Start = GetTime(eventcursor.GetLong(2)),
                        End = GetTime(eventcursor.GetLong(3))
                    };
                    result.Add(m);
                }
            }
            Meetings = result;
            return result;
        }

        private static DateTime GetTime(long val)
        {
            var res = new DateTime(val * 10000);
            res = res.AddYears(1969);
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            res = res.Add(offset);
            return res;
        }

        public void Open(long id, Object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;
            var uri = ContentUris.WithAppendedId(Events.ContentUri, id);
            var intent = new Intent(Intent.ActionView).SetData(uri);
            con.StartActivity(intent);
        }

        public Meeting GetEvent(Meeting meeting, Object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;
            var eventsUri = CalendarContract.Events.ContentUri;

            string[] eventsProjection = {
                    CalendarContract.Events.InterfaceConsts.Id,
                    CalendarContract.Events.InterfaceConsts.Title,
                    CalendarContract.Events.InterfaceConsts.Dtstart,
                    CalendarContract.Events.InterfaceConsts.Dtend
                 };

            var eventcursor = con.ManagedQuery(eventsUri, eventsProjection, $"_id={meeting.InternalID}", null, "dtstart ASC");

            while (eventcursor.MoveToNext())
            {
                var m = new Meeting()
                {
                    CalendarId = meeting.CalendarId,
                    InternalID = eventcursor.GetLong(0),
                    Title = eventcursor.GetString(1),
                    Start = GetTime(eventcursor.GetLong(2)),
                    End = GetTime(eventcursor.GetLong(3))
                };
                return m;
            }
            return null;
        }
    }
}