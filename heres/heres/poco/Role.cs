using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class Role : ItemBase
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
