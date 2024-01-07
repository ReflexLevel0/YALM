using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class ProcessInfo
{
	public CpuLog? CpuLog { get; set; }
	public MemoryLog? MemoryLog {get; set; }
	public IList<ProcessLog> ProcessLogs { get; set; } = new List<ProcessLog>();
	
	public override string ToString()
	{
		var builder = new StringBuilder(512);
		if (CpuLog != null)
		{
			builder.AppendLine($"Usage: {CpuLog.Usage:P}\nNumber of tasks: {CpuLog.NumberOfTasks}\n");
		}

		if (MemoryLog != null)
		{
			builder.AppendLine($"Memory: {MemoryLog.MemoryTotalKb}KB total, {MemoryLog.MemoryFreeKb}KB free, {MemoryLog.MemoryUsedKb}KB used, {MemoryLog.CachedKb}KB cached, {MemoryLog.AvailableMemoryKb}KB available\nSwap: {MemoryLog.SwapTotalKb}KB total, {MemoryLog.SwapFreeKb}KB free, {MemoryLog.SwapUsedKb}KB used");
		}
		
		foreach(var log in ProcessLogs)
		{
			builder.AppendLine(log.ToString());
		}
		builder.AppendLine();
		return builder.ToString();
	}
}