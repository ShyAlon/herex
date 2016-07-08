using System;
using System.Collections.Generic;
using System.Text;

using heres.poco;

namespace heres
{
    public interface ICalendar
    {
        IEnumerable<Meeting> GetEvents(Object context = null);
    }
}
