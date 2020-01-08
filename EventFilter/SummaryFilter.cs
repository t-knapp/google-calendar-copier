using Google.Apis.Calendar.v3.Data;

namespace GoogleCalendarCopier.EventFilter {
    public class SummaryFilter : IEventFilter
    {
        private string _containsValue;
        public SummaryFilter(string containsValue)
        {
            _containsValue = containsValue.ToLower();
        }
        public bool KeepEvent(Event calendarEvent)
        {
            return calendarEvent.Summary.ToLower().Equals(_containsValue);
        }
    }
}