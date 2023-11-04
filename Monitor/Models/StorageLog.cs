namespace Monitor.Models;

public class StorageLog
{
	public string UUID { get; }
	public string? Label { get; }
	public Filesystem Filesystem { get; }
	public string? MountPath { get; }
	public long? Bytes { get; }
	public double? UsedPercentage { get; }

	public StorageLog(string uuid, string? label, Filesystem filesystem, string? mountPath, long? bytes, double? usedPercentage)
	{
		UUID = uuid;
		Label = label;
		Filesystem = filesystem;
		MountPath = mountPath;
		Bytes = bytes;
		UsedPercentage = usedPercentage;
	}

	public override string ToString() => $"{UUID} {Label} {Filesystem} {MountPath} {Bytes} {UsedPercentage:P}";
}