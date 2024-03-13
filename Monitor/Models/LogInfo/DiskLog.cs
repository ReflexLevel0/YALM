namespace YALM.Monitor.Models.LogInfo;

public class DiskLog
{
	public string? Name { get; set; }
	public string? Path { get; set; }
	public string? PtType { get; set; }
	public string? Uuid { get; set; }
	public string? Serial { get; set; }
	public ulong? Size { get; set; }
	public string? State { get; set; }
	public string? Vendor { get; set; }
}