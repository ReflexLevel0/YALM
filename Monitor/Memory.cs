namespace Monitor;

public class Memory
{
	public double TotalMemoryMb { get; set; }
	public double UsedMemoryMb { get; set; }
	public double UsedPercentage { get; set; }
	public List<ProcessMemoryInfo> Processes { get; } = new();
}