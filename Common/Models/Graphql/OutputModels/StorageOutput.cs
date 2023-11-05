namespace Common.Models.Graphql.OutputModels;

public class StorageOutput : GraphqlModelBase
{
	public List<StorageVolume> Volumes { get; }
    
	public StorageOutput(int serverId, DateTime date, List<StorageVolume> volumes) : base(serverId, date)
	{
		Volumes = volumes;
	}
}