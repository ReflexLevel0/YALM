namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionInput
{
	public int ServerId { get; set; }
	public string DiskUuid { get; set; }
	public string Uuid { get; set; }
	public string? FilesystemName { get; set; }
	public string? FilesystemVersion { get; set; }
	public string? PartitionLabel { get; set; }
	public string? Mountpath { get; set; }

	public PartitionInput(int serverId, string diskUuid, string uuid)
	{
		ServerId = serverId;
		DiskUuid = diskUuid;
		Uuid = uuid;
	}

	public PartitionInput(int serverId, string diskUuid, string uuid, string? filesystemName, string? filesystemVersion, string? partitionLabel, string? mountpath)
	{
		ServerId = serverId;
		DiskUuid = diskUuid;
		Uuid = uuid;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		PartitionLabel = partitionLabel;
		Mountpath = mountpath;
	}
}