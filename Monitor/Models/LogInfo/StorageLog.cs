namespace YALM.Monitor.Models.LogInfo;

public class StorageLog
{
	public string? Uuid { get; set; }
	public string? Label { get; set; }
	public string? FilesystemName { get; set; }
	public string? FilesystemVersion { get; set; }
	public string? MountPath { get; set; }
	public long? Bytes { get; set; }
	public double? UsedPercentage { get; set; }

	public override string ToString() => $"{Uuid} {Label} {FilesystemName} {FilesystemVersion} {MountPath} {Bytes} {UsedPercentage:P}";
}