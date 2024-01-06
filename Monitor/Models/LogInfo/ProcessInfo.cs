using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class ProcessInfo
{
	public double? CpuUsage { get; set; }
	public int? NumberOfTasks { get; set; }
	public ulong? SwapFreeKb { get; set; }
	public ulong? SwapUsedKb { get; set; }
	public ulong? AvailableMemoryKb { get; set; }
	public ulong? MemoryFreeKb { get; set; }
	public ulong? MemoryUsedKb { get; set; }
	public ulong? CachedKb { get; set; }
	public IList<ProcessLog> ProcessLogs { get; set; } = new List<ProcessLog>();
	
	public override string ToString()
	{
		var builder = new StringBuilder(512);
		builder.AppendLine($"Usage: {CpuUsage:P}\nNumber of tasks: {NumberOfTasks}\n");
		builder.AppendLine($"Memory: {MemoryFreeKb}KB free, {MemoryUsedKb}KB used, {CachedKb}KB cached, {AvailableMemoryKb}KB available\nSwap: {SwapFreeKb}KB free, {SwapUsedKb}KB used");
		foreach(var log in ProcessLogs)
		{
			builder.AppendLine(log.ToString());
		}
		builder.AppendLine();
		return builder.ToString();
	}
}