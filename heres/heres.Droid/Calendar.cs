using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using heres.poco;
using Xamarin.Forms;
using Android.Provider;
using static Android.Provider.CalendarContract;
using Xamarin.Contacts;
using System.Text;

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

        private static Dictionary<long, string> contacts = new Dictionary<long, string>();

        private static Dictionary<string, Person> ContactsByEmail = new Dictionary<string, Person>();

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

            var cursor = con.ContentResolver.Query(calendarsUri, calendarsProjection, null, null, null);
            while (cursor.MoveToNext())
            {
                var calId = cursor.GetLong(PROJECTION_ID_INDEX);
                var displayName = cursor.GetString(PROJECTION_DISPLAY_NAME_INDEX);
                var accountName = cursor.GetString(PROJECTION_ACCOUNT_NAME_INDEX);

                var eventsUri = CalendarContract.Events.ContentUri;

                string[] eventsProjection = {
                    CalendarContract.Events.InterfaceConsts.Id,
                    CalendarContract.Events.InterfaceConsts.Title,
                    CalendarContract.Events.InterfaceConsts.Dtstart,
                    CalendarContract.Events.InterfaceConsts.Dtend
                 };

                var eventcursor = con.ContentResolver.Query(eventsUri, eventsProjection, $"calendar_id={calId}", null, "dtstart ASC");

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

            var uri = ContactsContract.Contacts.ContentUri;

            string[] projection =
            {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
            };

            cursor = con.ContentResolver.Query(uri, projection, null, null, null);

            if (cursor.MoveToFirst())
            {
                do
                {
                    contacts[cursor.GetLong(cursor.GetColumnIndex(projection[0]))] = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                } while (cursor.MoveToNext());
            }


            //var book = new AddressBook(con.ApplicationContext);
            //book.RequestPermission().ContinueWith(t =>
            //{
            //    if (!t.Result)
            //    {
            //        Console.WriteLine("Permission denied by user or manifest");
            //        return;
            //    }

            //    foreach (var c in book)
            //    {
            //        foreach (var e in c.Emails)
            //        {
            //            ContactsByEmail[e.Address] = c;
            //        }
            //    }

            //    Console.WriteLine("Finished");

            //}).Wait();

            return result;
        }

        public Person GetContact(string v, object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;

            var p = new Person();

            var uri = ContactsContract.CommonDataKinds.Email.ContentUri;

            string[] projection =
            {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.CommonDataKinds.Email.Address
            };

            var selection = $"{ContactsContract.CommonDataKinds.Email.InterfaceConsts.DisplayNamePrimary}='{v}'";

            var cursor = con.ContentResolver.Query(uri, projection, selection, null, null);

            if (cursor.MoveToFirst())
            {
                do
                {
                    return new Person
                    {
                        ContactID = cursor.GetLong(cursor.GetColumnIndex(projection[0])),
                        Name = cursor.GetString(cursor.GetColumnIndex(projection[1])),
                        Email = cursor.GetString(cursor.GetColumnIndex(projection[2])),
                    };
                } while (cursor.MoveToNext());
            }
            return null;

        }

        public IEnumerable<string> GetParticipantNames(object context)
        {
            return contacts.Values;
        }


        private static DateTime GetTime(long val)
        {
            var res = new DateTime(val * 10000);
            res = res.AddYears(1969);
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            res = res.Add(offset);
            return res;
        }

        private void GetContacts(Object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;
            var uri = ContactsContract.Contacts.ContentUri;
            //var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            string[] projection = { ContactsContract.Contacts.InterfaceConsts.Id,
            ContactsContract.Contacts.InterfaceConsts.DisplayName, //ContactsContract.CommonDataKinds.Phone.Number ,
            //ContactsContract.CommonDataKinds.Email.Address
            };
            //var cursor = ManagedQuery (uri, projection, null, null, null);
            con.RunOnUiThread(() =>
            {
                using (var loader1 = new CursorLoader(con.ApplicationContext, uri, projection, null, null, null))
                {
                    var cursor = (Android.Database.ICursor)loader1.LoadInBackground();
                    var contactList = new List<Contact>();
                    Android.Database.ICursor nestedCursor;
                    string id;
                    string phone = "";
                    var sb = new StringBuilder();

                    if (cursor.MoveToFirst())
                    {
                        do
                        {
                            id = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                            //id = cursor.GetString(cursor.GetColumnIndex(BaseColumns.Id));
                            //****load email address
                            using (var loader2 = new CursorLoader(con.ApplicationContext, ContactsContract.CommonDataKinds.Email.ContentUri, null,
                                ContactsContract.CommonDataKinds.Email.InterfaceConsts.ContactId + " = " + id,
                                //new String[]{ cursor.GetString(cursor.GetColumnIndex(BaseColumns.Id)) }
                                null, null))
                            {
                                nestedCursor = (Android.Database.ICursor)loader2.LoadInBackground();

                                if (nestedCursor.MoveToFirst())
                                {
                                    do
                                    {
                                        sb.Append(nestedCursor.GetString(
                                            nestedCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data)));
                                    } while (nestedCursor.MoveToNext());
                                }

                                //****load phones
                                using (var loader3 = new CursorLoader(con.ApplicationContext, ContactsContract.CommonDataKinds.Phone.ContentUri, null,
                                    ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = " + id,
                                    //new String[]{ cursor.GetString(cursor.GetColumnIndex(BaseColumns.Id)) }
                                    null, null))
                                {
                                    nestedCursor = (Android.Database.ICursor)loader3.LoadInBackground();

                                    if (nestedCursor.MoveToFirst())
                                    {
                                        phone = nestedCursor.GetString(
                                                nestedCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data));
                                    }

                                    //***
                                    var mail = sb.ToString();

                                    ContactsByEmail[mail] = new Person
                                    {
                                        Email = mail,
                                        Name = cursor.GetString(cursor.GetColumnIndex(projection[1]))
                                    };

                                    sb.Clear();
                                    phone = "";
                                }
                            }
                        } while (cursor.MoveToNext());
                    }
                }
            });
        }

        public void Open(long id, Object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;
            var uri = ContentUris.WithAppendedId(Events.ContentUri, id);
            var intent = new Intent(Intent.ActionView).SetData(uri);
            con.StartActivity(intent);
        }

        public Meeting GetEvent(Meeting meeting, object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;
            var eventsUri = CalendarContract.Events.ContentUri;

            string[] eventsProjection = {
                    Events.InterfaceConsts.Id,
                    Events.InterfaceConsts.Title,
                    Events.InterfaceConsts.Dtstart,
                    Events.InterfaceConsts.Dtend
                 };

            var eventcursor = con.ContentResolver.Query(eventsUri, eventsProjection, $"_id={meeting.InternalID}", null, "dtstart ASC");

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

        public IList<Person> GetParticipants(Meeting meeting, object context)
        {
            var con = context != null ? (Activity)context : (Activity)Forms.Context;
            var eventsUri = Attendees.ContentUri;
            var result = new List<Person>();

            string[] eventsProjection = {
                    Attendees.InterfaceConsts.Id,
                    Attendees.InterfaceConsts.EventId,
                    Attendees.InterfaceConsts.AttendeeName,
                    Attendees.InterfaceConsts.AttendeeEmail,
                    Attendees.InterfaceConsts.AttendeeIdentity,
                    Attendees.InterfaceConsts.AttendeeStatus,
                    Attendees.InterfaceConsts.IsOrganizer,
                 };

            var eventcursor = con.ContentResolver.Query(eventsUri, eventsProjection, $"event_id={meeting.InternalID}", null, null);

            while (eventcursor.MoveToNext())
            {
                var m = new Person()
                {
                    ContactID = eventcursor.GetLong(0),
                    ParentID = eventcursor.GetLong(1),
                    Name = eventcursor.GetString(2),
                    Email = eventcursor.GetString(3),
                    // Identity = eventcursor.GetString(4),
                    Status = eventcursor.GetString(5),
                    IsOrganizer = eventcursor.GetInt(6),
                };
                result.Add(m);
            }


            //string[] dataProjection = {  ContactsContract.CommonDataKinds.Phone.Number };
            //foreach (var p in result)
            //{
            //    var dataSelection = $" {ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId} = {p.ContactID} ";
            //    var phones = con.ContentResolver.Query(ContactsContract.Data.ContentUri, dataProjection, dataSelection, null, null);
            //    while (phones.MoveToNext())
            //    {
            //        var number = phones.GetString(0);
            //        p.PhoneNumber = number;
            //    }
            //}


            return result;
        }
    }
}