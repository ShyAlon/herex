using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class Person : ItemBase
    {
        public int IsOrganizer { get; internal set; }
        public string Name { get; set; }
        public long ContactID { get; internal set; }
        public string Email { get; internal set; }

        /// <summary>
        /// True when added by the calendar (can't be removed)
        /// </summary>
        [Ignore]
        public bool Organic { get; internal set; }

        //public string PhoneNumber { get; internal set; }
        //public long PhoneType { get; internal set; }
        //public string Photo { get; internal set; }
        //public string Identity { get; internal set; }

        public string Status { get; internal set; }

        /// <summary>
        /// Roles linked to the person
        /// </summary>
        [Ignore]
        public ICollection<Role> Roles { get; internal set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
