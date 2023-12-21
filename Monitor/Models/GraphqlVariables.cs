using YALM.Common.Models.Graphql.InputModels;

namespace YALM.Monitor.Models;

public class GraphqlVariables
{
	public CpuLogInput CpuLog;
	public MemoryLogInput MemoryLog;
	public PartitionLogInput PartitionLog;
}