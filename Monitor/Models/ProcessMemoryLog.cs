namespace Monitor.Models;

public class ProcessMemoryLog
{
	public string Name { get; }
	public double MemoryUsagePercentage { get; }
	
	public ProcessMemoryLog(string name, double memoryUsagePercentage)
	{
		Name = name;
		MemoryUsagePercentage = memoryUsagePercentage;
	}

	public override string ToString()
	{
		return $"{Name} {MemoryUsagePercentage:P} MEM";
	}
}