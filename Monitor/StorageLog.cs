namespace Monitor;

public class StorageLog
{
	public string Filesystem { get; }
	public string MountPath { get; }
	public double Bytes { get; }
	public double UsedBytes { get; }
	public double UsedPercentage => UsedBytes / Bytes;

	public StorageLog(string filesystem, string mountPath, double bytes, double usedBytes)
	{
		Filesystem = filesystem;
		MountPath = mountPath;
		Bytes = bytes;
		UsedBytes = usedBytes;
	}

	public override string ToString() => $"{Filesystem} {MountPath} {Bytes} {UsedBytes} {UsedPercentage:P}";
}