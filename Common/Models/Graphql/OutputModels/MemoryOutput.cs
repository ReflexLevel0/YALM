using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.OutputModels;

public class MemoryOutput : MemoryOutputBase, ILoggingBase<MemoryLog>
{
	public List<MemoryLog> Logs { get; } = new();
	
	public MemoryOutput(int serverId) : base(serverId)
	{
	}
}