using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public abstract class ItemBase
    {
        /// <summary>
        /// The id of the event in the database
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }

        /// <summary>
        /// The parent / container
        /// </summary>
        public long ParentID { get; set; }
    }

    public class Meeting : ItemBase
    {
        [Unique]
        public long InternalID { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        [Ignore]
        public DateTime StartDate
        {
            get
            {
                return Start.Date;
            }
            set
            {
                Start = value + StartTime;
            }
        }
        [Ignore]
        public DateTime EndDate
        {
            get
            {
                return End.Date;
            }
            set
            {
                End = value + EndTime;
            }
        }
        [Ignore]
        public TimeSpan StartTime
        {
            get
            {
                return Start.TimeOfDay;
            }
            set
            {
                Start = StartDate + value;
            }
        }
        [Ignore]
        public TimeSpan EndTime
        {
            get
            {
                return End.TimeOfDay;
            }
            set
            {
                End = End.Date + value;
            }
        }
        public long CalendarId { get; set; }
        public string StartString
        {
            get
            {
                return Start.ToShortDateString() + " " + Start.ToShortTimeString();
            }
        }
        [Ignore]
        public bool Tracked { get; set; }

        [Ignore]
        public ICollection<Person> Participants {get;set;}
    }
}
