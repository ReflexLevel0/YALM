using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.InputModels;

public class StorageInput : StorageLog, IDbLogBase
{
	public int ServerId { get; }
	public int Interval { get; }
	public string Uuid { get; }
	public string? Label { get; }
	public string? FilesystemName { get; }
	public string? FilesystemVersion { get; }
	public string? MountPath { get; }

	public StorageInput(int serverId, DateTime date, int interval, string uuid, string? label, string? filesystemName, string? filesystemVersion, string? mountPath, long? bytes, double? usedPercentage) : base(date, bytes, usedPercentage)
	{
		ServerId = serverId;
		Interval = interval;
		Uuid = uuid;
		Label = label;
		FilesystemName = filesystemName;
		FilesystemVersion = filesystemVersion;
		MountPath = mountPath;
	}
}