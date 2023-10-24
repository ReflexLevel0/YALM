using API.Models.Db;

namespace API;

using Npgsql;

public class Db : IDb
{
	private readonly NpgsqlDataSource _dataSource;

	public Db(string connString)
	{
		var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
		_dataSource = dataSourceBuilder.Build();
	}

	public async Task<NpgsqlConnection> OpenConnectionAsync() => await _dataSource.OpenConnectionAsync();

	public async IAsyncEnumerable<NpgsqlDataReader> ExecuteReaderAsync(string command)
	{
		await using var cmd = _dataSource.CreateCommand(command);
		await using var reader = await cmd.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			yield return reader;
		}
	}

	public async Task ExecuteNonQueryAsync(string command)
	{
		await using var cmd = _dataSource.CreateCommand(command);
		await cmd.ExecuteNonQueryAsync();
	}

	public async Task<object?> ExecuteScalarAsync(string command)
	{
		await using var cmd = _dataSource.CreateCommand(command);
		return await cmd.ExecuteScalarAsync();
	}

	public CpuLog ParseCpuRecord(NpgsqlDataReader reader)
	{
		int id = reader.GetInt32(0);
		var date = reader.GetDateTime(1);
		int logInterval = reader.GetInt32(2);
		double usage = reader.GetDouble(3);
		int numberOfTasks = reader.GetInt32(4);
		return new CpuLog(id, date, logInterval, usage, numberOfTasks);
	}
}