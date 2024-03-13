using Newtonsoft.Json;

namespace YALM.Monitor.Models.LogInfo;

public class PartitionInfo
{
	[JsonProperty("name")]
	public string? Name { get; set; }
	
	[JsonProperty("fstype")]
	public string? FilesystemType { get; set; }
	
	[JsonProperty("fsused")]
	public long? FilesystemUsed { get; set; }
	
	[JsonProperty("fsver")]
	public string? FilesystemVersion { get; set; }
	
	[JsonProperty("fsavail")]
	public long? FilesystemAvailable { get; set; }
	
	[JsonProperty("ptuuid")]
	public string DiskUuid { get; set; }
	
	[JsonProperty("partuuid")]
	public string PartitionUuid { get; set; }
	
	[JsonProperty("type")]
	public string? PartitionType { get; set; }
	
	[JsonProperty("size")]
	public long? Size { get; set; }
	
	[JsonProperty("mountpoint")]
	public string? Mountpoint { get; set; }
	
	[JsonProperty("path")]
	public string? Label { get; set; }
}