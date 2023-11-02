namespace Common.Models.Graphql;

public abstract class GraphqlModelBase
{
	public int ServerId { get; }
	public DateTime Date { get; set; }

	protected GraphqlModelBase(int serverId, DateTime date)
	{
		ServerId = serverId;
		Date = date;
	}
}