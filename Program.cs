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
using GoogleCalendarCopier.EventFilter;
using GoogleCalendarCopier.RequestBuilders;
using GoogleCalendarCopier.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IHost host = Host.CreateDefaultBuilder(args).Build();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

GoogleCalendarCopier.Settings settings = config.GetRequiredSection(nameof(Settings)).Get<GoogleCalendarCopier.Settings>();

string[] Scopes = { CalendarService.Scope.CalendarEvents };
string ApplicationName = "Google Calendar API .NET Quickstart";

IEventFilter[] filters = settings.SummaryValues.Select(value => new SummaryFilter(value)).ToArray();
filters = filters.Concat(settings.DescriptionValues.Select(value => new DescriptionFilter(value))).ToArray();

IEventFilter eventFilter = new OrFilter(filters);

if (!File.Exists(settings.CredentialsFilePath)) {
    Console.WriteLine($"{nameof(settings.CredentialsFilePath)} is not a file or does not exist.");
    return 1;
}

//////////////////////////////////
UserCredential credential;

using (var stream = new FileStream(settings.CredentialsFilePath, FileMode.Open, FileAccess.Read))
{
    string storagePath = Path.GetDirectoryName(settings.CredentialsFilePath);
    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        GoogleClientSecrets.Load(stream).Secrets,
        Scopes,
        "user",
        CancellationToken.None,
        new FileDataStore(storagePath, true));
    Console.WriteLine($"Credential file saved to: {storagePath}");
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
foreach (var sourceCalendar in settings.SourceCalendars)
{
    listRequestBuilder = new ListRequestBuilder(service, sourceCalendar);
    sourceEvents = listRequestBuilder.ListRequest.Execute();
    if (sourceEvents.Items != null && sourceEvents.Items.Count > 0)
        allSourceEvents.AddRange(sourceEvents.Items.Where((e) => eventFilter.KeepEvent(e)));
}

List<Event> allDestinationEvents = new List<Event>();
listRequestBuilder = new ListRequestBuilder(service, settings.DestinationCalendar);
listRequestBuilder.ShowDeleted(true);
Events destinationEvents = listRequestBuilder.ListRequest.Execute();
if (destinationEvents.Items != null && destinationEvents.Items.Count > 0)
    allDestinationEvents.AddRange(destinationEvents.Items);

IEnumerable<string> allDestinationEventIds = allDestinationEvents.Select(item => item.Id);
IList<Event> newEvents = allSourceEvents.Where(item => !allDestinationEventIds.Contains(item.Id)).ToList();

Console.WriteLine("Relevant Source Events");
PrintEvents(allSourceEvents);

Console.WriteLine();            
Console.WriteLine("Destination Events");
PrintEvents(allDestinationEvents);

Console.WriteLine();
Console.WriteLine("New Events to insert");
if (settings.DryRun)
    Console.WriteLine("Dry run is enabled, won't apply any changes.");
PrintEvents(newEvents);
if (!settings.DryRun)
    InsertEvents(service, newEvents, settings.DestinationCalendar);

void PrintEvents(IList<Event> events)
{
    foreach (var eventItem in events)
        Console.WriteLine("id: {0} {1} ({2}) Status: {3}", eventItem.Id, eventItem.Summary, eventItem.When(), eventItem.Status);
}

void InsertEvents(CalendarService service, IList<Event> events, string destinationCalendar)
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

return 0;