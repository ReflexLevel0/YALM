namespace Common.Models.Graphql;

public class StorageVolume
{
    public string Filesystem { get; }
    public string MountPath { get; }
    public long Bytes { get; }
    public long UsedBytes { get; }

    public StorageVolume(string filesystem, string mountPath, long bytes, long usedBytes)
    {
        Filesystem = filesystem;
        MountPath = mountPath;
        Bytes = bytes;
        UsedBytes = usedBytes;
    }
}