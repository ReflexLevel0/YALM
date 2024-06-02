namespace YALM.Common.Models.Graphql.OutputModels;

public class AlertOutput(int serverId, DateTimeOffset date, AlertSeverity severity, string text)
{
	public int ServerId { get; set; } = serverId;
	public DateTimeOffset Date { get; set; } = date;
	public AlertSeverity Severity { get; set; } = severity;
	public string Text { get; set; } = text;
}