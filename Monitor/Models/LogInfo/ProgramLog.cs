namespace YALM.Monitor.Models.LogInfo;

public class ProgramLog
{
	public string? Name { get; set; }
	public double? MemoryUsage { get; set; }
	public double? CpuUsage { get; set; }

	public override string ToString()
	{
		return $"{Name} {CpuUsage:P} CPU%, {MemoryUsage:P} MEM%";
	}
}