using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.OutputModels;

public class MemoryOutput : GraphqlModelBase<MemoryLog>
{
	public MemoryOutput(int serverId) : base(serverId)
	{
	}
}