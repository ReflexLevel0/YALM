namespace Monitor;

public class ProcessMemoryInfo
{
	public string Name { get; set; }
	public double MemoryMB { get; set; }
	
	public ProcessMemoryInfo(string name, double memoryMb)
	{
		Name = name;
		MemoryMB = memoryMb;
	}
}