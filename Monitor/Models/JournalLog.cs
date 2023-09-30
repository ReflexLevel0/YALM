namespace Monitor.Models;

public class JournalLog
{
	public DateTime Date { get; }
	public string Text { get; }
	public JournalLog(DateTime date, string text)
	{
		Date = date;
		Text = text;
	}

	public override string ToString() => $"{Date} {Text}";
}