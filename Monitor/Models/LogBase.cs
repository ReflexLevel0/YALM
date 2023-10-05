using System.Text;

namespace Monitor.Models;

public class LogBase
{
	public DateTime LogTime { get; }
	public CpuLog? CpuLog { get; set; }
	public MemoryLog? MemoryLog { get; set; }
	public List<StorageLog>? StorageLogs { get; set; }
	public List<ServiceLog>? ServiceLogs { get; set; }

	public LogBase(DateTime logTime)
	{
		LogTime = logTime;
	}
	
	public override string ToString()
	{
		var builder = new StringBuilder(2048);
		builder.AppendLine($"Log time: {LogTime}\n");
		if (CpuLog != null) builder.AppendLine(CpuLog.ToString());
		if (MemoryLog != null) builder.AppendLine(MemoryLog.ToString());
		if (StorageLogs != null)
		{
			StorageLogs.ForEach(l => builder.AppendLine(l.ToString()));
			builder.AppendLine();
		}
		ServiceLogs?.ForEach(l => builder.AppendLine(l.ToString()));
		return builder.ToString();
	}
}