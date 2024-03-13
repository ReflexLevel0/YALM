using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.OutputModels;

public class PartitionOutput : PartitionOutputBase, ILoggingBase<PartitionLog>
{
	public List<PartitionLog> Logs { get; } = new();
	
	public PartitionOutput(int serverId, string uuid, string? filesystemName, string? filesystemVersion, string? label, string? mountPath) : base(serverId, uuid, filesystemName, filesystemVersion, label, mountPath)
	{
	}
}