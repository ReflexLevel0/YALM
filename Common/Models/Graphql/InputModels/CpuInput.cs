namespace YALM.Common.Models.Graphql.InputModels;

public class CpuInput
{
	public int ServerId { get; set; }
	public string? Name { get; set; }
	public string? Architecture { get; set; }
	public int? Cores { get; set; }
	public int? Threads { get; set; }
	public int? FrequencyMhz { get; set; }
}