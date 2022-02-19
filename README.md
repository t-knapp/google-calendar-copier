# google-calendar-copier

Reads events from multiple source calendars. Filters events based on event summary and description. Inserts filtered events to one destination calendar.

## Build
```
dotnet publish
```

## Run
```
dotnet google-calendar-copier.dll
```
or
```
./google-calendar-copier
```

## Configuration
Configuration file in JSON format is provided as first CLI argument.

```
{   
    "Settings": {
        "SourceCalendars": ["<google-calendar-id>", "<google-calendar-id>"],
        "SummaryValues": ["Stand-Up", "Team-Meeting", "Kick-Off"],
        "DescriptionValues": ["Your Name", "Your Mention", "Some Text"],
        "DestinationCalendar": "<google-calendar-id>",
        "CredentialsFilePath": "<absolute-path-to-credentials-file-for-google-api>",
        "DryRun": false
    }
}
```
`SummaryValues` are checked with equals.
`DescriptionValues` are checked with contains.
Both filter are logical OR combined.

## Development
```
dotnet run
```