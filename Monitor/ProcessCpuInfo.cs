namespace Monitor;

public class ProcessCpuInfo
{
	public string Name { get; set; }
	public double CpuUsage { get; set; }

	public ProcessCpuInfo(string name, double cpuUsage)
	{
		Name = name;
		CpuUsage = cpuUsage;
	}
}