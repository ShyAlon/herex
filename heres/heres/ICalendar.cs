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
        IEnumerable<Meeting> GetEvents(object context = null);

        /// <summary>
        /// Opens the event in the  calendar application
        /// </summary>
        void Open(long id, Object context = null);

        ///Get a meeting
        Meeting GetEvent(Meeting meeting, object context = null);

        /// <summary>
        /// Get the participants of an event
        /// </summary>
        /// <param name="meeting">The event</param>
        /// <returns>A list of participants</returns>
        IList<Person> GetParticipants(Meeting meeting, object context = null);
    }
}
