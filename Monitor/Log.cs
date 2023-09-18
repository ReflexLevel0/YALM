namespace Monitor;

public class Log
{
	public DateTime Date { get; }
	public string Text { get; }

	public Log(DateTime date, string text)
	{
		Date = date;
		Text = text;
	}

	public override string ToString()
	{
		return $"{Date} {Text}";
	}
}