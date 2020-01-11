using System;
using Google.Apis.Calendar.v3.Data;

namespace GoogleCalendarCopier.Extensions {
    public static class EventExtensions {
        public static string When(this Event e)
        {
            string when = e.Start.DateTime.ToString();
            if (String.IsNullOrEmpty(when))
            {
                when = e.Start.Date;
            }
            return when;
        }
    }
}