using YALM.Common.Models.Graphql.InputModels;

namespace YALM.Monitor.Models;

public class GraphqlVariables
{
	public CpuInput? Cpu { get; set; }
	public CpuLogInput? CpuLog { get; set; }
	public MemoryLogInput? MemoryLog { get; set; }
	public PartitionLogInput? PartitionLog { get; set; }
	public List<ProgramLogInput>? ProgramLogs { get; set; }
	public List<DiskInput>? Disks { get; set; }
	public List<PartitionInput>? Partitions { get; set; }
	public List<PartitionLogInput>? PartitionLogs { get; set; }
}