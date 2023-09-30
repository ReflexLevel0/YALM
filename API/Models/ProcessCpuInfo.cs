namespace API.Models;

public class ProcessCpuInfo
{
	public string Name { get; }
	public double CpuUsage { get; }

	public ProcessCpuInfo(string name, double cpuUsage)
	{
		Name = name;
		CpuUsage = cpuUsage;
	}

	public override string ToString()
	{
		return $"{Name}: {CpuUsage:P} CPU";
	}
}