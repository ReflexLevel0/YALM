using Newtonsoft.Json;

namespace YALM.Monitor.Models.CpuJSON;

public class LscpuJson
{
	[JsonProperty("lscpu")]
	public List<CpuFieldJson>? Fields { get; set; }
}