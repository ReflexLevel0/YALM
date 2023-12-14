namespace Common.Models.Graphql;

public record Payload<TLog>
{
	public TLog? Log { get; set; }
	public string? Error { get; set; }
}