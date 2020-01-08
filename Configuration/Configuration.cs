using System.Collections.Generic;

namespace GoogleCalendarCopier.Configuration {
    public class Configuration
    {
        public IList<string> SourceCalendars {get; set;}
        public Configuration() {
            SourceCalendars = new List<string>();
        }
    }
}