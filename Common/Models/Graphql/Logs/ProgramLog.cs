namespace YALM.Common.Models.Graphql.Logs;

public class ProgramLog : LogBase
{
	public string? Name { get; set; }
	public double? CpuUsage { get; set; }
	public double? MemoryUsage { get; set; }
}