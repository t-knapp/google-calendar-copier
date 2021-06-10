# google-calendar-copier

Reads events from multiple source events. Filters events based on event summary and description. Inserts filtered events to one destination calendar.

## Usage
dotnet run <config.json-file-path> <oauth-tokens-folder-path/>

## Configuration

Configuration file in JSON format is provided as first CLI argument.

```
{
    "SourceCalendars": ["<google-calendar-id>", "<google-calendar-id>"],
    "SummaryValues": ["Stand-Up", "Team-Meeting", "Kick-Off"],
    "DescriptionValues": ["Your Name", "Your Mention", "Some Text"],
    "DestinationCalendar": "<google-calendar-id>"
}
```
`SummaryValues` are checked with equals.
`DescriptionValues` are checked with contains.