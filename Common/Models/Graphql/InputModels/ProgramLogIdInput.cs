namespace YALM.Common.Models.Graphql.InputModels;

public class ProgramLogIdInput(int serverId, string name, DateTime date)
{
	public int ServerId { get; } = serverId;
	public string Name { get; } = name;
	public DateTime Date { get; } = date;
}