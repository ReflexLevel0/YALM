namespace YALM.Common.Models.Graphql.Logs;

public class CpuLog : LogBase
{
	public double? Usage { get; set; }
	public int? NumberOfTasks { get; set; }
}