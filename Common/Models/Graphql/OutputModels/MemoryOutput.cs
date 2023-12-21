using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.OutputModels;

public class MemoryOutput : MemoryOutputBase, ILoggingBase<MemoryLog>
{
	public List<MemoryLog> Logs { get; } = new();
	
	public MemoryOutput(int serverId) : base(serverId)
	{
	}
}