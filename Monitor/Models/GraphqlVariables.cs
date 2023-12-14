using Common.Models.Graphql.InputModels;

namespace Monitor.Models;

public class GraphqlVariables
{
	public CpuLogInput CpuLog;
	public MemoryLogInput MemoryLog;
	public PartitionLogInput PartitionLog;
}