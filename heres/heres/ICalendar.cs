using System;
using System.Collections.Generic;
using System.Text;

using heres.poco;

namespace heres
{
    public interface ICalendar
    {
        /// <summary>
        /// Get the relevant events
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<Meeting> GetEvents(Object context = null);

        /// <summary>
        /// Opens the event in the  calendar application
        /// </summary>
        void Open(long id, Object context = null);

        ///Get a meeting
        Meeting GetEvent(Meeting meeting, Object context = null);
    }
}
