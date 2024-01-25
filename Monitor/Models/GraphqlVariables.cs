using YALM.Common.Models.Graphql.InputModels;

namespace YALM.Monitor.Models;

public class GraphqlVariables
{
	public CpuIdInput? OldCpu;
	public CpuInput? Cpu;
	public CpuLogInput? CpuLog;
	public MemoryLogInput? MemoryLog;
	public PartitionLogInput? PartitionLog;
	public List<ProgramLogInput>? ProgramLogs;
}