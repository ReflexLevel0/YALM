namespace API;

using Npgsql;

public class Db
{
	private readonly NpgsqlDataSource _dataSource;

	public Db(string connString)
	{
		var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
		_dataSource = dataSourceBuilder.Build();
	}

	public async Task<NpgsqlConnection> OpenConnectionAsync() => await _dataSource.OpenConnectionAsync();

	public async IAsyncEnumerable<NpgsqlDataReader> ExecuteReadAsync(string command)
	{
		await using var cmd=_dataSource.CreateCommand(command);
		await using var reader = await cmd.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			yield return reader;
		}
	}
}