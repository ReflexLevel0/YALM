using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class LogBase(DateTime logTime)
{
	public DateTime LogTime { get; } = logTime;
	public CpuInfo? CpuInfo { get; set; }
	public ProgramInfo? ProgramInfo { get; set; }
	public List<StorageLog>? StorageLogs { get; set; }
	public List<ServiceLog>? ServiceLogs { get; set; }

	public override string ToString()
	{
		var builder = new StringBuilder(2048);
		builder.AppendLine($"Log time: {LogTime}\n");
		if (CpuInfo != null) builder.AppendLine(CpuInfo.ToString());
		if (ProgramInfo != null) builder.AppendLine(ProgramInfo.ToString());
		if (StorageLogs != null)
		{
			StorageLogs.ForEach(l => builder.AppendLine(l.ToString()));
			builder.AppendLine();
		}
		ServiceLogs?.ForEach(l => builder.AppendLine(l.ToString()));
		return builder.ToString();
	}
}