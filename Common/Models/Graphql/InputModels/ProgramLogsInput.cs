using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.InputModels;

public class ProgramLogsInput
{
	public List<ProcessLog> ProgramLogs { get; } = new();
}