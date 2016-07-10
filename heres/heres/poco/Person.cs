using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class Person : IItem
    {
        public long ContactID { get; internal set; }
        public string Email { get; internal set; }
        public long ID
        {
            get;
            set;
        }
        public string Identity { get; internal set; }
        public int IsOrganizer { get; internal set; }
        public long MeetingID { get; internal set; }
        public string Name { get; set; }
        public string PhoneNumber { get; internal set; }
        public long PhoneType { get; internal set; }
        public string Status { get; internal set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
