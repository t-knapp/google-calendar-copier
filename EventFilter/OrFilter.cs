using System.Linq;
using Google.Apis.Calendar.v3.Data;

namespace GoogleCalendarCopier.EventFilter {
    public class OrFilter : IEventFilter
    {
        private IEventFilter[] _filters;
        public OrFilter(params IEventFilter[] filters) 
        {
            _filters = filters;
        }
        
        public bool KeepEvent(Event calendarEvent)
        {
            return _filters.Any(filter => filter.KeepEvent(calendarEvent));
        }
    }
}