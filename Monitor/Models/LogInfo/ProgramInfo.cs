using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class ProgramInfo
{
	public CpuLog? CpuLog { get; set; }
	public MemoryLog? MemoryLog {get; set; }
	public List<ProgramLog>? ProgramLogs { get; set; }
	
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

		if (ProgramLogs != null)
		{
			foreach(var log in ProgramLogs)
			{
				builder.AppendLine(log.ToString());
			}	
		}
		
		builder.AppendLine();
		return builder.ToString();
	}
}