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

        //public string PhoneNumber { get; internal set; }
        //public long PhoneType { get; internal set; }
        //public string Photo { get; internal set; }
        //public string Identity { get; internal set; }

        public string Status { get; internal set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
