using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoogleCalendarCopier.Configuration;
using GoogleCalendarCopier.EventFilter;
using GoogleCalendarCopier.EventSource;

namespace GoogleCalendarCopier
{
    class Program
    {
        static string[] Scopes = { CalendarService.Scope.CalendarEvents };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: dotnet run <configfilePath>");
                return;
            }

            IConfigurationProvider configurationProvider = new JsonConfigurationProvider(args[0]);
            var configuration = await configurationProvider.Read();

            IEventFilter[] filters = configuration.SummaryValues.Select(value => new SummaryFilter(value)).ToArray();
            filters = filters.Concat(configuration.DescriptionValues.Select(value => new DescriptionFilter(value))).ToArray();

            IEventFilter eventFilter = new OrFilter(filters);

            //////////////////////////////////
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            Events sourceEvents;
            ListRequestBuilder listRequestBuilder;
            List<Event> allSourceEvents = new List<Event>();
            foreach (var sourceCalendar in configuration.SourceCalendars)
            {
                listRequestBuilder = new ListRequestBuilder(service, sourceCalendar);
                EventsResource.ListRequest request = listRequestBuilder.ListRequest;
                sourceEvents = request.Execute();
                if (sourceEvents.Items != null && sourceEvents.Items.Count > 0)
                    allSourceEvents.AddRange(sourceEvents.Items.Where((e) => eventFilter.KeepEvent(e)));
            }

            foreach (var eventItem in allSourceEvents)
            {
                string when = eventItem.Start.DateTime.ToString();
                if (String.IsNullOrEmpty(when))
                {
                    when = eventItem.Start.Date;
                }
                Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                //insertRequest = service.Events.Insert(eventItem, configuration.DestinationCalendar);
                //insertRequest.Execute();
                //Console.WriteLine("\t{0}", eventItem.Description);
            }
        }
    }
}
