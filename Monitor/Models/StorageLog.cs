namespace Monitor.Models;

public class StorageLog
{
	public string Filesystem { get; }
	public string MountPath { get; }
	public long Bytes { get; }
	public long UsedBytes { get; }
	public double UsedPercentage => (double)UsedBytes / Bytes;

	public StorageLog(string filesystem, string mountPath, long bytes, long usedBytes)
	{
		Filesystem = filesystem;
		MountPath = mountPath;
		Bytes = bytes;
		UsedBytes = usedBytes;
	}

	public override string ToString() => $"{Filesystem} {MountPath} {Bytes} {UsedBytes} {UsedPercentage:P}";
}