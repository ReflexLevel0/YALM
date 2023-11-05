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
		int id = reader.GetInt32(0);
		var date = reader.GetDateTime(1);
		int logInterval = reader.GetInt32(2);
		string uuid = reader.GetString(3);
		string label = reader.GetString(4);
		string filesystemName = reader.GetString(5);
		string filesystemVersion = reader.GetString(6);
		string mountpath = reader.GetString(7);
		long bytesTotal = reader.GetInt64(8);
		double usedPercentage = reader.GetDouble(9);
		return new StorageDbLog(id, date, logInterval, uuid, label, filesystemName, filesystemVersion, mountpath, bytesTotal, usedPercentage);
	}
}