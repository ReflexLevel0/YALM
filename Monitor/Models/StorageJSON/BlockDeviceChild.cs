using Newtonsoft.Json;

namespace YALM.Monitor.Models.StorageJSON;

public class BlockDeviceChild
{
	public string? Name { get; set; }
	public string? FsType { get; set; }
	public string? FsVer { get; set; }
	public string? Label { get; set; }
	public string? Uuid { get; set; }
	public string? FsAvail { get; set; }
	[JsonProperty("fsuse%")]
	public string? FsUse { get; set; }
	public List<string> Mountpoints { get; set; }
}