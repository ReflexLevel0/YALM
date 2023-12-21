namespace YALM.Monitor.Models;

public class StorageLog
{
	public string UUID { get; }
	public string? Label { get; }
	public string? FilesystemName { get; }
	public string? FilesystemVersion { get; }
	public string? MountPath { get; }
	public long? Bytes { get; }
	public double? UsedPercentage { get; }

	public StorageLog(string uuid, string? label, string? filesystemName, string? filesystemVersion, string? mountPath, long? bytes, double? usedPercentage)
	{
		UUID = uuid;
		Label = label;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		MountPath = mountPath;
		Bytes = bytes;
		UsedPercentage = usedPercentage;
	}

	public override string ToString() => $"{UUID} {Label} {FilesystemName} {FilesystemVersion} {MountPath} {Bytes} {UsedPercentage:P}";
}