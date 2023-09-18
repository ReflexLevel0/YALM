using System.Text;

namespace Monitor;

public class MemoryLog
{
	public double TotalMemoryMb { get; set; }
	public double UsedMemoryMb { get; set; }
	public double UsedPercentage { get; set; }
	public List<ProcessMemoryLog> Processes { get; } = new();

	public override string ToString()
	{
		var builder = new StringBuilder(512);
		builder.AppendLine($"Memory: {TotalMemoryMb}MB total, {UsedMemoryMb}MB used, {UsedPercentage:P} used");
		foreach (var process in Processes)
		{
			builder.AppendLine(process.ToString());
		}
		
		return builder.ToString();
	}
}