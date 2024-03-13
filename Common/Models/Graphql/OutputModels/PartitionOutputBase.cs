namespace YALM.Common.Models.Graphql.OutputModels;

public class PartitionOutputBase
{
	public int ServerId { get; }
	public string Uuid { get; }
	public string? FilesystemName { get; }
	public string? FilesystemVersion { get; }
	public string? Label { get; }
	public string? MountPath { get; }
	
	public PartitionOutputBase(int serverId, string uuid, string? filesystemName, string? filesystemVersion, string? label, string? mountPath)
	{
		ServerId = serverId;
		Uuid = uuid;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		Label = label;
		MountPath = mountPath;
	}
}