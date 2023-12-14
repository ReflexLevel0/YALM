namespace Common.Models.Graphql;

public abstract class GraphqlModelBase<T> where T : LogBase
{
	public int ServerId { get; }
	public List<T> Logs { get; } = new();

	protected GraphqlModelBase(int serverId)
	{
		ServerId = serverId;
	}
}