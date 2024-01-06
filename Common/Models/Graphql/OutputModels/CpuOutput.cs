using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.OutputModels;

public class CpuOutput : CpuOutputBase, ILoggingBase<CpuLog>
{
	public List<CpuLog> Logs { get; } = new();

	public CpuOutput(int serverId) : base(serverId)
	{
		
	}

	public CpuOutput(int serverId, string? name, string? architecture, int? cores, int? threads, int? frequency) : base(serverId, name, architecture, cores, threads, frequency)
	{
		
	}
}