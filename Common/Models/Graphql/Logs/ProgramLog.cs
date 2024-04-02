namespace YALM.Common.Models.Graphql.Logs;

public class ProgramLog : LogBase
{
	public required string Name { get; set; }
	public decimal? CpuUsage { get; set; }
	public decimal? MemoryUsage { get; set; }
}