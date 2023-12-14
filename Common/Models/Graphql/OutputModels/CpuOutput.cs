using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.OutputModels;

public class CpuOutput : GraphqlModelBase<CpuLog>
{
	public CpuOutput(int serverId) : base(serverId)
	{
	}
}