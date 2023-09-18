namespace Monitor;

public class ProcessMemoryInfo
{
	public string Name { get; set; }
	public double MemoryUsagePercentage { get; set; }
	
	public ProcessMemoryInfo(string name, double memoryUsagePercentage)
	{
		Name = name;
		MemoryUsagePercentage = memoryUsagePercentage;
	}

	public override string ToString()
	{
		return $"{Name} {MemoryUsagePercentage:P} MEM";
	}
}