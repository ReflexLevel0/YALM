namespace YALM.Common.Models.Graphql.OutputModels;

public class CpuOutputBase
{
	public string? Name { get; set; }
	public string? Architecture { get; set; }
	public int? Cores { get; set; }
	public int? Threads { get; set; }
	public int? Frequency { get; set; }
	public int ServerId { get; set; }

	public CpuOutputBase(int serverId)
	{
		ServerId = serverId;
	}
	
	public CpuOutputBase(int serverId, string? name, string? architecture, int? cores, int? threads, int? frequency)
	{
		ServerId = serverId;
		Name = name;
		Architecture = architecture;
		Cores = cores;
		Threads = threads;
		Frequency = frequency;
	}
}