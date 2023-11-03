namespace API.Models.Db;

public class StorageLog : LogBase
{
	public string Filesystem { get; }
	public string Mountpath { get; }
	public int BytesTotal { get; }
	public int UsedBytes { get; }
	
	[GraphQLIgnore]
	public double Usage => (double)UsedBytes / BytesTotal;

	public StorageLog(int serverId, DateTime date, int interval, string filesystem, string mountpath, int bytesTotal, int usedBytes) : base(serverId, date, interval)
	{
		Filesystem = filesystem;
		Mountpath = mountpath;
		BytesTotal = bytesTotal;
		UsedBytes = usedBytes;
	}
}