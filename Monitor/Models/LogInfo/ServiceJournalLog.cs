namespace YALM.Monitor.Models.LogInfo;

public class ServiceJournalLog
{
	public DateTime Date { get; }
	public string Text { get; }
	public ServiceJournalLog(DateTime date, string text)
	{
		Date = date;
		Text = text;
	}

	public override string ToString() => $"{Date} {Text}";
}