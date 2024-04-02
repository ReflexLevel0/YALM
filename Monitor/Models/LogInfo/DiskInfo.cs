using Newtonsoft.Json;

namespace YALM.Monitor.Models.LogInfo;

public class DiskInfo
{
	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("path")]
	public string? Path { get; set; }

	[JsonProperty("ptuuid")]
	public string DiskUuid { get; set; }

	[JsonProperty("size")]
	public long? Bytes { get; set; }

	[JsonProperty("pttype")]
	public string? DiskType { get; set; }
	
	[JsonProperty("serial")]
	public string? Serial { get; set; }
	
	[JsonProperty("vendor")]
	public string? Vendor { get; set; }
	
	[JsonProperty("children")]
	public List<PartitionInfo>? Children { get; set; }
}