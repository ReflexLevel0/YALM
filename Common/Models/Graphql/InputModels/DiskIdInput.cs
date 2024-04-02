namespace YALM.Common.Models.Graphql.InputModels;

public class DiskIdInput
{
	public int ServerId { get; set; }
	public string Uuid { get; set; }
	
	public DiskIdInput(int serverId, string uuid)
	{
		ServerId = serverId;
		Uuid = uuid;
	}
}