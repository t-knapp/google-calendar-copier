using System.Collections.Generic;

namespace GoogleCalendarCopier.Configuration {
    public class Configuration
    {
        public IList<string> SourceCalendars {get; set;}
        public IList<string> SummaryValues {get; set;}
        public IList<string> DescriptionValues {get; set;}
        public Configuration() {
            SourceCalendars = new List<string>();
            SummaryValues = new List<string>();
            DescriptionValues = new List<string>();
        }
    }
}