using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public interface IID
    {
        long ID { get; set; }
    }

    public abstract class ItemBase : Google.Apis.Requests.IDirectResponseSchema, IID
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
        [Newtonsoft.Json.JsonPropertyAttribute("parentId")]
        public long ParentID { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }

    public class Meeting : ItemBase
    {
        [Unique]
        [Newtonsoft.Json.JsonPropertyAttribute("internalId")]
        public long InternalID { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("title")]
        public string Title { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("originator")]
        public string Originator { get; set; }

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
        public bool Tracked { get { return ID > 0; } }

        [Ignore]
        public ICollection<Person> Participants {get;set;}

        public override string ToString()
        {
            return $"{ID}: {Title} ({StartString})";
        }
    }
}
