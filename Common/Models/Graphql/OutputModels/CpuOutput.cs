using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.OutputModels;

public class CpuOutput : CpuOutputBase, ILoggingBase<CpuLog>
{
	public List<CpuLog> Logs { get; } = new();
	
	public CpuOutput(int serverId) : base(serverId)
	{
	}
}