namespace YALM.Monitor.Models;

public class ProcessCpuLog
{
	public string Name { get; }
	public double CpuUsage { get; }

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