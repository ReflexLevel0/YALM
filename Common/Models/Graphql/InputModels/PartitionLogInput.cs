using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionLogInput : PartitionLog, ILog
{
	public int ServerId { get; set; }
	public int Interval { get; set; }
	public string? Uuid { get; set; }
}