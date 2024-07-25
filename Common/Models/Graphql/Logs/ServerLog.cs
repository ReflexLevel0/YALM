namespace YALM.Common.Models.Graphql.Logs;

public class ServerLog : LogBase
{
	public int ServerId { get; set; }
	public int Interval { get; set; }
}