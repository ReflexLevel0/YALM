namespace YALM.Common.Models.Graphql.InputModels;

public class DiskInput
{
	public int ServerId { get; }
	public string Label { get; }

	public DiskInput(int serverId, string label)
	{
		ServerId = serverId;
		Label = label;
	}
}