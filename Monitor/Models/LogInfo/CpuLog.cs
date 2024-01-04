using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class CpuLog
{
	public double Usage { get; set; }
	public int NumberOfTasks { get; set; }

	public override string ToString()
	{
		var builder = new StringBuilder(512);
		builder.AppendLine($"CPU usage: {Usage:P}\nNumber of tasks: {NumberOfTasks}");
		return builder.ToString();
	}
}