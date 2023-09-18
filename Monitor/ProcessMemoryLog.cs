namespace Monitor;

public class ProcessMemoryLog
{
	public string Name { get; set; }
	public double MemoryUsagePercentage { get; set; }
	
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