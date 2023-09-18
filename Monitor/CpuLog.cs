using System.Text;

namespace Monitor;

public class CpuLog
{
	public double Usage { get; set; }
	public int NumberOfTasks { get; set; }
	public List<ProcessCpuLog> Processes { get; } = new();

	public override string ToString()
	{
		var builder = new StringBuilder(512);
		builder.AppendLine($"CPU usage: {Usage:P}\nNumber of tasks: {NumberOfTasks}");
		foreach (var process in Processes)
		{
			builder.AppendLine(process.ToString());
		}
		
		return builder.ToString();
	}
}