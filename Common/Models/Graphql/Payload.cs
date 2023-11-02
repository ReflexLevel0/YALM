namespace Common.Models.Graphql;

public record Payload<TLog> where TLog : GraphqlModelBase
{
	public TLog? Log { get; set; }
	public string? Error { get; set; }
}