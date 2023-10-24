namespace Common.Models.Graphql;

public class GraphqlModelBase
{
	public int ServerId { get; }
	public DateTime Date { get; }

	protected GraphqlModelBase(int serverId, DateTime date)
	{
		ServerId = serverId;
		Date = date;
	}
}