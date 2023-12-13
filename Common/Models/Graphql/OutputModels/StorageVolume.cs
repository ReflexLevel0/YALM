namespace Common.Models.Graphql.OutputModels;

public class StorageVolume : GraphqlModelBase
{
    public string UUID { get; }
    public string? Label { get; }
    public string? FilesystemName { get; }
    public string? FilesystemVersion { get; }
    public string? MountPath { get; }
    public long? Bytes { get; }
    public double? UsedPercentage { get; }

    public StorageVolume(int serverId, DateTime date, string uuid, string? label, string? filesystemName, string? filesystemVersion, string? mountPath, long? bytes, double? usedPercentage) : base(serverId, date)
    {
        UUID = uuid;
        Label = label;
        FilesystemName = filesystemName;
        FilesystemVersion = filesystemVersion;
        MountPath = mountPath;
        Bytes = bytes;
        UsedPercentage = usedPercentage;
    }
}