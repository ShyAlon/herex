using System;
using System.Collections.Generic;
using System.Text;

namespace heres.poco
{
    public class CollectionOf<T>
    {
        public List<T> items { get; set; }
        public string kind { get; set; }
        public string etag { get; set; }
    }
}
