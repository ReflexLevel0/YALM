namespace YALM.Common.Models.Graphql.Logs;

public class MemoryLog : LogBase
{
	public int? MbUsed { get; set; }
	public int? MbTotal { get; set; }
}