namespace YALM.Common.Models.Graphql.InputModels;

public class CpuIdInput
{
	public int ServerId { get; set; }

	public CpuIdInput(int serverId)
	{
		ServerId = serverId;
	}
}