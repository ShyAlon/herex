using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class Meeting
    {
        public long Id { get; set; }
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
