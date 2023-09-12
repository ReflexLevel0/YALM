namespace Monitor;

public class Cpu
{
	public double Usage { get; set; }
	public List<ProcessCpuInfo> Processes { get; } = new();
}