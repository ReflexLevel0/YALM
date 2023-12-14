namespace Common.Models.Graphql.OutputModels;

public class DiskOutput : DiskOutputBase
{
	public List<PartitionOutput> Partitions { get; set; } = new();
	
	public DiskOutput(int serverId, string? label) : base(serverId, label)
	{
	}
}