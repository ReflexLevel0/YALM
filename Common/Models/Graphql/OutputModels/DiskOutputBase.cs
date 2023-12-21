namespace YALM.Common.Models.Graphql.OutputModels;

public class DiskOutputBase
{
	public int ServerId { get; set; }
	public string? Label { get; set; }
	
	public DiskOutputBase(int serverId, string? label)
	{
		ServerId = serverId;
		Label = label;
	}
}