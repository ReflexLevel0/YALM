using API.Models.Db;
using Common.Models.Graphql.InputModels;
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

		await foreach (var log in QueryHelper.GetLogs(_db, "Cpu", sqlSelectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, startDateTime, endDateTime, interval))
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
		
		await foreach(var log in QueryHelper.GetLogs(_db, "Memory", selectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, startDateTime, endDateTime, interval))
		{
			yield return log;
		}
	}

	// public async IAsyncEnumerable<StorageInput> Storage(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	// {
	// 	string selectQuery = $"SELECT serverid, date, interval, filesystem, mountpath, bytestotal, usedbytes " +
	// 	                     $"FROM storage " +
	// 	                     $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
	// 	                     $"ORDER BY filesystem,date";
	//
	// 	Func<IList<StorageLog>, StorageInput> combineLogsFunc = logs =>
	// 	{
	// 		long usedBytes = (long)QueryHelper.CombineValues(method, logs.Select(s => s.UsedBytes).ToList());
	// 		long totalBytes = (long)QueryHelper.CombineValues(method, logs.Select(s => s.BytesTotal).ToList());
	// 		return new StorageInput(serverId, logs.First().Date, new[]
	// 		{
	// 			new StorageVolume(logs.First().Filesystem, logs.First().Mountpath, totalBytes, usedBytes)
	// 		});
	// 	};
	// 	
	// 	Func<NpgsqlDataReader, StorageLog> parseRecordFunc = reader => _db.ParseStorageRecord(reader);
	// 	var getEmptyRecordFunc = () => new StorageInput(serverId, DateTime.Now, new List<StorageVolume>());
	// 	
	// 	await foreach(var log in QueryHelper.GetLogs(_db, "Storage", selectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, startDateTime, endDateTime, interval))
	// 	{
	// 		yield return log;
	// 	}
	// }
}