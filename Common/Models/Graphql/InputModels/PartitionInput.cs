namespace Common.Models.Graphql.InputModels;

public class PartitionInput
{
	public string DiskLabel { get; }
	public string Uuid { get; }
	public string FilesystemName { get; }
	public string FilesystemVersion { get; }
	public string PartitionLabel { get; }
	public string Mountpath { get; }

	public PartitionInput(string diskLabel, string uuid, string filesystemName, string filesystemVersion, string partitionLabel, string mountpath)
	{
		DiskLabel = diskLabel;
		Uuid = uuid;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		PartitionLabel = partitionLabel;
		Mountpath = mountpath;
	}
}