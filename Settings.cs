using System.Collections.Generic;

namespace GoogleCalendarCopier {
    public class Settings
    {
        public IEnumerable<string> SourceCalendars {get; set;}
        public IEnumerable<string> SummaryValues {get; set;}
        public IEnumerable<string> DescriptionValues {get; set;}
        public string DestinationCalendar {get; set;}
        public string CredentialsFilePath {get; set;}
        public bool DryRun {get; set;}
    }
}