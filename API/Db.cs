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

	public CpuDbLog ParseCpuLogRecord(NpgsqlDataReader reader)
	{
		int id = reader.GetInt32(0);
		var date = reader.GetDateTime(1);
		int logInterval = reader.GetInt32(2);
		double usage = reader.GetDouble(3);
		int numberOfTasks = reader.GetInt32(4);
		return new CpuDbLog(id, date, logInterval, usage, numberOfTasks);
	}

	public MemoryDbLog ParseMemoryLogRecord(NpgsqlDataReader reader)
	{
		int id = reader.GetInt32(0);
		var date = reader.GetDateTime(1);
		int logInterval = reader.GetInt32(2);
		int mbUsed = reader.GetInt32(3);
		int mbTotal = reader.GetInt32(4);
		return new MemoryDbLog(id, date, logInterval, mbUsed, mbTotal);
	}

	public PartitionDbLog ParsePartitionLogRecord(NpgsqlDataReader reader)
	{
		int diskId = reader.GetInt32(0);
		string uuid = reader.GetString(1);
		var date = reader.GetDateTime(2);
		int logInterval = reader.GetInt32(3);
		long bytesTotal = reader.GetInt64(4);
		double usedPercentage = reader.GetDouble(5);
		return new PartitionDbLog(diskId, date, logInterval, uuid, bytesTotal, usedPercentage);
	}

	public PartitionDb ParsePartitionRecord(NpgsqlDataReader reader)
	{
		string uuid = reader.GetString(1);
		string filesystemName = reader.GetString(2);
		string filesystemVersion = reader.GetString(3);
		string label = reader.GetString(4);
		string mountPath = reader.GetString(4);
		return new PartitionDb(uuid, filesystemName, filesystemVersion, label, mountPath);
	}

	public DiskDb ParseDiskRecord(NpgsqlDataReader reader)
	{
		int serverId = reader.GetInt32(0);
		string label = reader.GetString(1);
		return new DiskDb(serverId, label);
	}
}