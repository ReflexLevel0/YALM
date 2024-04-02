using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class MemoryInfo
{
	public ulong? MemoryTotalKb { get; set; }
	public ulong? SwapTotalKb { get; set; }

	public override string ToString()
	{
		var builder = new StringBuilder(512);
		builder.AppendLine($"Memory: {MemoryTotalKb}KB total");
		builder.AppendLine($"Swap: {SwapTotalKb}KB total");
		return builder.ToString();
	}
}