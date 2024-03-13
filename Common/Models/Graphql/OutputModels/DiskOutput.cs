namespace YALM.Common.Models.Graphql.OutputModels;

public class DiskOutput : DiskOutputBase
{
	public List<PartitionOutput> Partitions { get; set; } = new();
	
	public DiskOutput(int serverId, string uuid, string? type, string? serial, string? path, string? vendor, string? model, long? bytesTotal) : base(serverId, uuid, type, serial, path, vendor, model, bytesTotal)
	{
	}
}