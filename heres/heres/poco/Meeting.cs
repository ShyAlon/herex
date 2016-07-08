using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class Meeting
    {
        /// <summary>
        /// The id of the event in the database
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The ID the phone gives to the event
        /// </summary>
        public long InternalID { get; set; }
        public string Title { get; set; }
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
    }
}
