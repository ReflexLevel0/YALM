using System.Text;

namespace Monitor;

public class ServiceLog
{
	public string Name { get; }
	public string Active { get; set; }
	public int Tasks { get; set; }
	public string Memory { get; set; }
	public string Cpu { get; set; }
	public List<Log> Logs { get; } = new();

	public ServiceLog(string name)
	{
		Name = name;
	}

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