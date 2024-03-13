namespace YALM.Common.Models.Graphql.InputModels;

public class DiskInput
{
	public required int ServerId { get; set; }
	public required string Uuid { get; set; }
	public string? Type { get; set; }
	public string? Serial { get; set; }
	public string? Path { get; set; }
	public string? Vendor { get; set; }
	public string? Model { get; set; }
	public long? BytesTotal { get; set; }
}