using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class Person : ItemBase
    {
        [Newtonsoft.Json.JsonIgnore]
        public int IsOrganizer { get; internal set; }
        [Newtonsoft.Json.JsonIgnore]
        public long ContactID { get; internal set; }
        
        [Newtonsoft.Json.JsonPropertyAttribute("email")]
        public string Email { get; internal set; }

        private string _name;
        // Can't be null
        [Newtonsoft.Json.JsonPropertyAttribute("name")]
        public virtual string Name
        {
            get { return string.IsNullOrEmpty(_name) ? "Unknown" : _name; }
            set { _name = value; }
        }


        /// <summary>
        /// True when added by the calendar (can't be removed)
        /// </summary>
        [Ignore]
        [Newtonsoft.Json.JsonIgnore]
        public bool Organic { get; internal set; }

        //public string PhoneNumber { get; internal set; }
        //public long PhoneType { get; internal set; }
        //public string Photo { get; internal set; }
        //public string Identity { get; internal set; }

        [Newtonsoft.Json.JsonIgnore]
        public string Status { get; internal set; }

        /// <summary>
        /// Roles linked to the person
        /// </summary>
        [Ignore]
        [Newtonsoft.Json.JsonIgnore]
        public ICollection<Role> Roles { get; internal set; }

        public override string ToString()
        {
            return Name;
        }
    }

}
