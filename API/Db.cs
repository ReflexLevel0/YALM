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

	public CpuDbLog ParseCpuRecord(NpgsqlDataReader reader)
	{
		int id = reader.GetInt32(0);
		var date = reader.GetDateTime(1);
		int logInterval = reader.GetInt32(2);
		double usage = reader.GetDouble(3);
		int numberOfTasks = reader.GetInt32(4);
		return new CpuDbLog(id, date, logInterval, usage, numberOfTasks);
	}

	public MemoryDbLog ParseMemoryRecord(NpgsqlDataReader reader)
	{
		int id = reader.GetInt32(0);
		var date = reader.GetDateTime(1);
		int logInterval = reader.GetInt32(2);
		int mbUsed = reader.GetInt32(3);
		int mbTotal = reader.GetInt32(4);
		return new MemoryDbLog(id, date, logInterval, mbUsed, mbTotal);
	}

	public StorageDbLog ParseStorageRecord(NpgsqlDataReader reader)
	{
		// int id = reader.GetInt32(0);
		// var date = reader.GetDateTime(1);
		// int logInterval = reader.GetInt32(2);
		// string filesystem = reader.GetString(3);
		// string mountpath = reader.GetString(4);
		// long bytesTotal = reader.GetInt64(5);
		// long bytesUsed = reader.GetInt64(6);
		// return new StorageLog(id, date, logInterval, filesystem, mountpath, bytesTotal, bytesUsed);
		return null;
	}
}