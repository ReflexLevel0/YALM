using API.Models.Db;
using Common.Models.Graphql.OutputModels;
using Npgsql;

namespace API;

public class Query
{
	private readonly IDb _db;

	public Query(IDb db)
	{
		_db = db;
	}

	/// <summary>
	/// Returns cpu logs
	/// </summary>
	/// <param name="serverId"></param>
	/// <param name="startDateTime"></param>
	/// <param name="endDateTime"></param>
	/// <param name="interval">Interval that specifies time distance between two logs. If interval is null, then interval is decided dynamically (interval=1 minute for every hour between <param name="startDateTime"></param> and <param name="endDateTime"></param>).</param>
	/// <param name="method">Method for combining multiple logs into one (min, max, avg, etc.)</param>
	/// <returns></returns>
	public async IAsyncEnumerable<CpuOutput> Cpu(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		string sqlSelectQuery = $"SELECT serverid, date, interval, usage, numberoftasks " +
		                        $"FROM Cpu " +
		                        $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
		                        $"ORDER BY date";
		
		Func<IList<CpuDbLog>, CpuOutput> combineLogsFunc = logs =>
		{
			double usageProcessed = QueryHelper.CombineValues(method, logs.Select(c => c.Usage).ToList());
			int numberOfTasksProcessed = (int)QueryHelper.CombineValues(method, logs.Select(c => (double)c.NumberOfTasks).ToList());
			return new CpuOutput(serverId, logs.First().Date, usageProcessed, numberOfTasksProcessed);
		};
		
		Func<NpgsqlDataReader, CpuDbLog> parseRecordFunc = reader => _db.ParseCpuRecord(reader);
		var getEmptyRecordFunc = () => new CpuOutput(serverId, DateTime.Now, 0, 0);

		await foreach (var log in QueryHelper.GetLogs(_db, "Cpu", sqlSelectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			yield return log;
		}
	}

	/// <summary>
	/// Returns memory logs
	/// </summary>
	/// <param name="serverId"></param>
	/// <param name="startDateTime"></param>
	/// <param name="endDateTime"></param>
	/// <param name="interval">Interval that specifies time distance between two logs. If interval is null, then interval is decided dynamically (interval=1 minute for every hour between <param name="startDateTime"></param> and <param name="endDateTime"></param>).</param>
	/// <param name="method">Method for combining multiple logs into one (min, max, avg, etc.)</param>
	/// <returns></returns>
	public async IAsyncEnumerable<MemoryOutput> Memory(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		string selectQuery = $"SELECT serverid, date, interval, mbused, mbtotal " +
		                     $"FROM Memory " +
		                     $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
		                     $"ORDER BY date";

		Func<IList<MemoryDbLog>, MemoryOutput> combineLogsFunc = logs =>
		{
			int mbused = (int)QueryHelper.CombineValues(method, logs.Select(l => (double)l.MbUsed).ToList());
			int mbtotal = (int)QueryHelper.CombineValues(method, logs.Select(l => (double)l.MbTotal).ToList());
			return new MemoryOutput(serverId, logs.First().Date, mbused, mbtotal);
		};
		
		Func<NpgsqlDataReader, MemoryDbLog> parseRecordFunc = reader => _db.ParseMemoryRecord(reader);
		var getEmptyRecordFunc = () => new MemoryOutput(serverId, DateTime.Now, 0, 0);
		
		await foreach(var log in QueryHelper.GetLogs(_db, "Memory", selectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			yield return log;
		}
	}

	/// <summary>
	/// Returns logs for the specified storage volume
	/// </summary>
	/// <param name="uuid">UUID of the partition for which logs are being returned</param>
	/// <returns></returns>
	public async IAsyncEnumerable<StorageVolume> StorageVolume(int serverId, string uuid, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		await foreach (var volume in GetStorageVolumesForUuid(serverId, uuid, startDateTime, endDateTime, interval, method))
		{
			yield return volume;
		}
	}

	/// <summary>
	/// Returns logs for all partitions
	/// </summary>
	/// <returns></returns>
	public async IAsyncEnumerable<StorageOutput> Storage(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		//Getting all UUID's and then returning data for each one of them
		await foreach (var reader in _db.ExecuteReaderAsync("SELECT DISTINCT uuid from storagelog"))
		{
			string uuid = reader.GetString(0);
			var volumes = new List<StorageVolume>();
			await foreach (var volume in GetStorageVolumesForUuid(serverId, uuid, startDateTime, endDateTime, interval, method))
			{
				volumes.Add(volume);
			}

			yield return new StorageOutput(serverId, volumes.First().Date, volumes);
		}
	}
	
	/// <summary>
	/// Returns storage logs for the specified partition
	/// </summary>
	private async IAsyncEnumerable<StorageVolume> GetStorageVolumesForUuid(int serverId, string uuid, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		string selectQuery = $"SELECT serverid, date, interval, sl.uuid, label, filesystemname, filesystemversion, mountpath, bytestotal, usage " +
		                     $"FROM storagelog sl " +
		                     $"JOIN partition p ON sl.uuid = p.uuid " +
		                     $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} AND sl.uuid='{uuid}' " +
		                     $"ORDER BY date";
	
		Func<IList<StorageDbLog>, StorageVolume> combineLogsFunc = logs =>
		{
			var firstLog = logs.First();
			long usedPercentage = (long)QueryHelper.CombineValues(method, logs.Select(s => s.UsedPercentage).ToList());
			long totalBytes = (long)QueryHelper.CombineValues(method, logs.Select(s => s.Bytes).ToList());
			return new StorageVolume(serverId, firstLog.Date, firstLog.UUID, firstLog.Label, firstLog.FilesystemName, firstLog.FilesystemVersion, firstLog.MountPath, totalBytes, usedPercentage);
		};
		
		Func<NpgsqlDataReader, StorageDbLog> parseRecordFunc = reader => _db.ParseStorageRecord(reader);
		var getEmptyRecordFunc = () => new StorageVolume(serverId, DateTime.Now, "", "", "", "", "", 0, 0);

		await foreach(var log in QueryHelper.GetLogs(_db, "StorageLog", selectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			yield return log;
		}
	}
}