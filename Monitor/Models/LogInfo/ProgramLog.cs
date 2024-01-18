namespace YALM.Monitor.Models.LogInfo;

public class ProgramLog(string name)
{
	public string Name { get; init; } = name;
	public decimal? MemoryUsage { get; set; }
	public decimal? CpuUsage { get; set; }

	public override string ToString()
	{
		return $"{Name} {CpuUsage:P} CPU%, {MemoryUsage:P} MEM%";
	}
}