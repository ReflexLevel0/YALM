using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.InputModels;

public class ProgramLogsInput
{
	public List<ProgramLog> ProgramLogs { get; } = new();
}