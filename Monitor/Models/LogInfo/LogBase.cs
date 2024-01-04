using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class LogBase(DateTime logTime)
{
	public DateTime LogTime { get; } = logTime;
	public CpuInfo? CpuInfo { get; set; }
	public CpuLog? CpuLog { get; set; }
	public MemoryInfo? MemoryInfo { get; set; }
	public List<ProcessLog>? ProcessLogs { get; set; }
	public List<StorageLog>? StorageLogs { get; set; }
	public List<ServiceLog>? ServiceLogs { get; set; }

	public override string ToString()
	{
		var builder = new StringBuilder(2048);
		builder.AppendLine($"Log time: {LogTime}\n");
		if (CpuInfo != null) builder.AppendLine(CpuInfo.ToString());
		if (CpuLog != null) builder.AppendLine(CpuLog.ToString());
		if (MemoryInfo != null) builder.AppendLine(MemoryInfo.ToString());
		if (ProcessLogs != null)
		{
			ProcessLogs.ForEach(p => builder.AppendLine(p.ToString()));
			builder.AppendLine();
		}
		if (StorageLogs != null)
		{
			StorageLogs.ForEach(l => builder.AppendLine(l.ToString()));
			builder.AppendLine();
		}
		ServiceLogs?.ForEach(l => builder.AppendLine(l.ToString()));
		return builder.ToString();
	}
}