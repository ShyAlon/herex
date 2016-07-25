using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class Role : ItemBase
    {
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// The importance of the role - 0 means negligable and ushort.Max means critical
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("importance")]
        public virtual int Importance { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("name")]
        public virtual string Name { get; set; }
    }
}
