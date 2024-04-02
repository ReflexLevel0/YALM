using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.OutputModels;

public class ProgramOutput
{
	public int ServerId { get; set; }
	public List<ProgramLog> Logs { get; } = new();

	public ProgramOutput(int serverId)
	{
		ServerId = serverId;
	}
}