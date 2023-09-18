namespace Monitor;

public class ProcessCpuLog
{
	public string Name { get; set; }
	public double CpuUsage { get; set; }

	public ProcessCpuLog(string name, double cpuUsage)
	{
		Name = name;
		CpuUsage = cpuUsage;
	}

	public override string ToString()
	{
		return $"{Name}: {CpuUsage:P} CPU";
	}
}