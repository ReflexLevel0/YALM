namespace Common.Models.Graphql.OutputModels;

public class StorageOutput : GraphqlModelBase
{
	public List<StorageVolume> StorageVolumes { get; }
    
	public StorageOutput(int serverId, DateTime date, IEnumerable<StorageVolume> storageVolumes) : base(serverId, date)
	{
		StorageVolumes = storageVolumes.ToList();
	}
}