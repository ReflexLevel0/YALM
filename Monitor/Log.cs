using System.Text;

namespace Monitor;

public class Log
{
	public DateTime LogTime { get; }
	public CpuLog? CpuLog { get; set; }
	public MemoryLog? MemoryLog { get; set; }
	public List<ServiceLog> ServiceLogs { get; } = new();

	public Log(DateTime logTime)
	{
		LogTime = logTime;
	}
	
	public override string ToString()
	{
		var builder = new StringBuilder(2048);
		builder.AppendLine($"Log time: {LogTime}\n");
		builder.AppendLine($"{CpuLog}\n{MemoryLog}\n");
		foreach (var service in ServiceLogs)
		{
			builder.AppendLine(service.ToString());
		}

		return builder.ToString();
	}
}