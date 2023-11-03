namespace Common.Models.Graphql;

public class Storage : GraphqlModelBase
{
    public List<StorageVolume> StorageVolumes { get; }
    
    public Storage(int serverId, DateTime date, IEnumerable<StorageVolume> storageVolumes) : base(serverId, date)
    {
        StorageVolumes = storageVolumes.ToList();
    }
}