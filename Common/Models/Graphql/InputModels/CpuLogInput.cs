using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.InputModels;

public class CpuLogInput : CpuLog, ILog
{
	public int ServerId { get; set; }
	public int Interval { get; set; }
}