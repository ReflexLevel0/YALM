namespace Common.Models.Graphql;

public class StorageVolume
{
    public string Filesystem { get; }
    public string MountPath { get; }
    public int Bytes { get; }
    public int UsedBytes { get; }

    public StorageVolume(string filesystem, string mountPath, int bytes, int usedBytes)
    {
        Filesystem = filesystem;
        MountPath = mountPath;
        Bytes = bytes;
        UsedBytes = usedBytes;
    }
}