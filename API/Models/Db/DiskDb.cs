namespace API.Models.Db;

public class DiskDb
{
	public int ServerId { get; }
	public string Label { get; }
	
	public DiskDb(int serverId, string label)
	{
		ServerId = serverId;
		Label = label;
	}
}