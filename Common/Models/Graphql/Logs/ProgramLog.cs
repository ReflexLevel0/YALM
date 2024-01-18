namespace YALM.Common.Models.Graphql.Logs;

public class ProgramLog : LogBase
{
	public int ServerId { get; set; }
	public int Interval { get; set; }
	public required string Name { get; set; }
	public decimal? CpuUsage { get; set; }
	public decimal? MemoryUsage { get; set; }
}