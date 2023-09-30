namespace API.Models;

public class Storage : Log
{
	public string Filesystem { get; }
	public string Mountpath { get; }
	public double BytesTotal { get; }
	public double UsedBytes { get; }
	public double Usage => UsedBytes / BytesTotal;

	public Storage(int serverId, DateTime date, int interval, string filesystem, string mountpath, double bytesTotal, double usedBytes) : base(serverId, date, interval)
	{
		Filesystem = filesystem;
		Mountpath = mountpath;
		BytesTotal = bytesTotal;
		UsedBytes = usedBytes;
	}
}