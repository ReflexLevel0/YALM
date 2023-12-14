using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.OutputModels;

public class StorageOutput : GraphqlModelBase<StorageLog>
{
	public string UUID { get; set; }
	public string? Label { get; set; }
	public string? FilesystemName { get; set; }
	public string? FilesystemVersion { get; set; }
	public string? MountPath { get; set; }

	public StorageOutput(int serverId) : base(serverId)
	{
		
	}
	
	public StorageOutput(int serverId, string uuid, string? label, string? filesystemName, string? filesystemVersion, string? mountPath) : base(serverId)
	{
		UUID = uuid;
		Label = label;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		MountPath = mountPath;
	}
}