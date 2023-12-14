namespace Common.Models.Graphql.InputModels;

public class DiskInput
{
	public int ServerId { get; }
	public string Label { get; }
	public List<PartitionInput>? Partitions { get; }

	public DiskInput(int serverId, string label, List<PartitionInput>? partitions)
	{
		ServerId = serverId;
		Label = label;
		Partitions = partitions;
	}
}