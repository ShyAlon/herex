using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public abstract class ItemBase : Google.Apis.Requests.IDirectResponseSchema
    {
        /// <summary>
        /// The id of the event in the database
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [Newtonsoft.Json.JsonPropertyAttribute("id")]
        public long ID { get; set; }

        /// <summary>
        /// The parent / container
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }

    public class Meeting : ItemBase
    {
        [Unique]
        public long InternalID { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("title")]
        public string Title { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("startTime")]
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        public long CalendarId { get; set; }
        public string StartString
        {
            get
            {
                return StartTime.ToShortDateString() + " " + StartTime.ToShortTimeString();
            }
        }
        [Ignore]
        public bool Tracked { get; set; }

        [Ignore]
        public ICollection<Person> Participants {get;set;}
    }


    public class MeetingsApiMeetingsCollection : Google.Apis.Requests.IDirectResponseSchema
    {
        [Newtonsoft.Json.JsonPropertyAttribute("items")]
        public virtual System.Collections.Generic.IList<Meeting> Items { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }
}
