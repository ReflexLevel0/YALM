namespace YALM.Common.Models.Graphql;

public record Payload<TData>
{
	public TData? Data { get; set; }
	public string? Error { get; set; }
}