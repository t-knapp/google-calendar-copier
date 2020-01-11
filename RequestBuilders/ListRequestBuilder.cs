using System;
using Google.Apis.Calendar.v3;

namespace GoogleCalendarCopier.RequestBuilders {
    public class ListRequestBuilder {
        public EventsResource.ListRequest ListRequest { private set; get; }

        public ListRequestBuilder(CalendarService calendarService, string calendarId)
        {
            ListRequest = calendarService.Events.List(calendarId);
            ListRequest.TimeMin = DateTime.Now;
            ListRequest.TimeMax = DateTime.Now.AddMonths(3);
            ListRequest.ShowDeleted = false;
            ListRequest.SingleEvents = true;
            ListRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
        }

        public ListRequestBuilder ShowDeleted(bool showDeleted)
        {
            ListRequest.ShowDeleted = showDeleted;
            return this;
        }

        public ListRequestBuilder TimeMax(DateTime maxTime)
        {
            ListRequest.TimeMax = maxTime;
            return this;
        }

        public ListRequestBuilder TimeMin(DateTime minTime)
        {
            ListRequest.TimeMin = minTime;
            return this;
        }

        public ListRequestBuilder MaxResults(int maxResults)
        {
            ListRequest.MaxResults = maxResults;
            return this;
        }
    }
}