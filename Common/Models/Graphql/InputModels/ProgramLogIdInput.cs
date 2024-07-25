namespace YALM.Common.Models.Graphql.InputModels;

public class ProgramLogIdInput(int serverId, string name, DateTimeOffset date)
{
	public int ServerId { get; } = serverId;
	public string Name { get; } = name;
	public DateTimeOffset Date { get; } = date;
}