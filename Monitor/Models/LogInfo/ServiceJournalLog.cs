namespace YALM.Monitor.Models.LogInfo;

public class ServiceJournalLog
{
	public DateTimeOffset Date { get; }
	public string Text { get; }
	public ServiceJournalLog(DateTimeOffset date, string text)
	{
		Date = date;
		Text = text;
	}

	public override string ToString() => $"{Date} {Text}";
}