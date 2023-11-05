using Common.Models.Graphql.OutputModels;

namespace Common.Models.Graphql.InputModels;

public class StorageInput : StorageOutput, IDbLogBase
{
	public int Interval { get; }

	public StorageInput(int serverId, DateTime date, int interval, List<StorageVolume> volumes) : base(serverId, date, volumes)
	{
		Interval = interval;
	}
}