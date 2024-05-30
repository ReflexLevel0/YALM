namespace YALM.Common.Models.Graphql.OutputModels;

public class AlertOutput(int serverId, DateTime date, string text)
{
	public int ServerId { get; set; } = serverId;
	public DateTime Date { get; set; } = date;
	public string Text { get; set; } = text;
}