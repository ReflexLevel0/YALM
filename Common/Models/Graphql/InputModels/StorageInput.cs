using Common.Models.Graphql.OutputModels;

namespace Common.Models.Graphql.InputModels;

public class StorageInput : StorageOutput, IDbLogBase
{
	public int Interval { get; }
	
	public StorageInput(int serverId, DateTime date, int interval, IEnumerable<StorageVolume> storageVolumes) : base(serverId, date, storageVolumes)
	{
		Interval = interval;
	}
}