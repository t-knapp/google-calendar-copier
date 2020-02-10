using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoogleCalendarCopier.Configuration;
using GoogleCalendarCopier.EventFilter;
using GoogleCalendarCopier.RequestBuilders;
using GoogleCalendarCopier.Extensions;

namespace GoogleCalendarCopier
{
    class Program
    {
        static string[] Scopes = { CalendarService.Scope.CalendarEvents };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: dotnet run <configfilePath> <credentialsFolderPath>");
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
                new FileStream(args[1] + "credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = args[1];
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
                sourceEvents = listRequestBuilder.ListRequest.Execute();
                if (sourceEvents.Items != null && sourceEvents.Items.Count > 0)
                    allSourceEvents.AddRange(sourceEvents.Items.Where((e) => eventFilter.KeepEvent(e)));
            }

            List<Event> allDestinationEvents = new List<Event>();
            listRequestBuilder = new ListRequestBuilder(service, configuration.DestinationCalendar);
            listRequestBuilder.ShowDeleted(true);
            Events destinationEvents = listRequestBuilder.ListRequest.Execute();
            if (destinationEvents.Items != null && destinationEvents.Items.Count > 0)
                allDestinationEvents.AddRange(destinationEvents.Items);
            
            IList<Event> newEvents = allSourceEvents.Except(allDestinationEvents).ToList();

            Console.WriteLine("Relevant Source Events");
            PrintEvents(allSourceEvents);

            Console.WriteLine();            
            Console.WriteLine("Destination Events");
            PrintEvents(allDestinationEvents);

            Console.WriteLine();
            Console.WriteLine("New Events to insert");
            PrintEvents(newEvents);

            Console.WriteLine();
            Console.WriteLine("Inserted Events");
            InsertEvents(service, newEvents, configuration.DestinationCalendar);
        }

        private static void PrintEvents(IList<Event> events)
        {
            foreach (var eventItem in events)
                Console.WriteLine("id: {0} {1} ({2}) Status: {3}", eventItem.Id, eventItem.Summary, eventItem.When(), eventItem.Status);
        }

        private static void InsertEvents(CalendarService service, IList<Event> events, string destinationCalendar)
        {
            if (events.Count == 0)
                Console.WriteLine("No Events to insert.");

            Event insertedEvent;
            EventsResource.InsertRequest request;
            foreach (var e in events)
            {
                try {
                    Console.WriteLine("Inserting: id: {0} {1} ({2})", e.Id, e.Summary, e.When());
                    request = service.Events.Insert(e, destinationCalendar);
                    insertedEvent = request.Execute();
                    Console.WriteLine("Inserted: id: {0} {1} ({2})", insertedEvent.Id, insertedEvent.Summary, insertedEvent.When());
                } catch (Exception ex) {
                    Console.WriteLine("Exception inserting Event: {0}", ex.Message);
                }
            }
        }
    }
}
