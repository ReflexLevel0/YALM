using System.Text;

namespace Monitor.Models;

public class ServiceLog
{
	public string? Name { get; init; }
	public string? Active { get; set; }
	public int Tasks { get; set; }
	public string? Memory { get; set; }
	public string? Cpu { get; set; }
	public List<JournalLog> Logs { get; } = new();

	public override string ToString()
	{
		var builder = new StringBuilder(2048);
		builder.AppendLine($"Service: {Name}\nActive: {Active}\nTasks: {Tasks}\nMemory: {Memory}\nCpu: {Cpu}");
		foreach (var log in Logs)
		{
			builder.AppendLine(log.ToString());
		}

		return builder.ToString();
	}
}