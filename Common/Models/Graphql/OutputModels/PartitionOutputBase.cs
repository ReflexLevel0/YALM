namespace Common.Models.Graphql.OutputModels;

public class PartitionOutputBase
{
	public string Uuid { get; }
	public string FilesystemName { get; }
	public string FilesystemVersion { get; }
	public string Label { get; }
	public string MountPath { get; }
	
	public PartitionOutputBase(string uuid, string filesystemName, string filesystemVersion, string label, string mountPath)
	{
		Uuid = uuid;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		Label = label;
		MountPath = mountPath;
	}
}