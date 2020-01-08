using Google.Apis.Calendar.v3.Data;

namespace GoogleCalendarCopier.EventFilter {
    public class DescriptionFilter : IEventFilter
    {
        private string _containsValue;
        public DescriptionFilter(string containsValue)
        {
            _containsValue = containsValue.ToLower();
        }
        public bool KeepEvent(Event calendarEvent)
        {
            return calendarEvent.Description != null && calendarEvent.Description.ToLower().Contains(_containsValue);
        }
    }
}