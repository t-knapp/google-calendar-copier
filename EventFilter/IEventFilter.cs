using System.Collections.Generic;
using Google.Apis.Calendar.v3.Data;

namespace GoogleCalendarCopier.EventFilter {
    public interface IEventFilter
    {
        bool KeepEvent(Event calendarEvent);
    }
}