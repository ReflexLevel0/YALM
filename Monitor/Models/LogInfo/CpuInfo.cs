using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class CpuInfo
{
	public string? Name { get; set; }
	public string? Architecture { get; set; }
	public int? Cores { get; set; }
	public int? Threads { get; set; }
	public int? Frequency { get; set; }
	public double? CpuUsage { get; set; }
	public int? NumberOfTasks { get; set; }

	public override string ToString()
	{
		var builder = new StringBuilder(512);
		builder.AppendLine($"Usage: {CpuUsage:P}\nNumber of tasks: {NumberOfTasks}\nName: {Name}\nArchitecture: {Architecture}\nCores: {Cores}\nThreads: {Threads}\nFrequency: {Frequency}MHz");
		return builder.ToString();
	}
}