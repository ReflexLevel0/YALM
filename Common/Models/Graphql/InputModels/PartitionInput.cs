namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionInput : PartitionIdInput
{
	public string? DiskUuid { get; }
	public string? FilesystemName { get; }
	public string? FilesystemVersion { get; }
	public string? PartitionLabel { get; }
	public string? Mountpath { get; }

	public PartitionInput(int serverId, string uuid) : base(serverId, uuid)
	{
	}

	public PartitionInput(int serverId, string diskUuid, string uuid, string? filesystemName, string? filesystemVersion, string? partitionLabel, string? mountpath) : base(serverId, uuid)
	{
		DiskUuid = diskUuid;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		PartitionLabel = partitionLabel;
		Mountpath = mountpath;
	}
}