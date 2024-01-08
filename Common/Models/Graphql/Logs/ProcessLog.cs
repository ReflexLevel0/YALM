namespace YALM.Common.Models.Graphql.Logs;

public class ProcessLog : LogBase
{
	public int ServerId { get; set; }
	public string? Name { get; set; }
	public double? CpuUsage { get; set; }
	public double? MemoryUsage { get; set; }
}