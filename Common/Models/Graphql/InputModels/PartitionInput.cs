namespace Common.Models.Graphql.InputModels;

public class PartitionInput
{
	public string Uuid { get; }
	public string FilesystemName { get; }
	public string FilesystemVersion { get; }
	public string Label { get; }
	public string Mountpath { get; }

	public PartitionInput(string uuid, string filesystemName, string filesystemVersion, string label, string mountpath)
	{
		Uuid = uuid;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		Label = label;
		Mountpath = mountpath;
	}
}