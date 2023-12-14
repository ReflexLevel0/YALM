using API.Models.Db;
using Common.Models.Graphql.Logs;
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
	public async Task<CpuOutput> Cpu(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		string sqlSelectQuery = $"SELECT serverid, date, interval, usage, numberoftasks " +
		                        $"FROM Cpu " +
		                        $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
		                        $"ORDER BY date";
		
		Func<IList<CpuDbLog>, CpuLog> combineLogsFunc = logs =>
		{
			double usageProcessed = QueryHelper.CombineValues(method, logs.Select(c => c.Usage).ToList());
			int numberOfTasksProcessed = (int)QueryHelper.CombineValues(method, logs.Select(c => (double)c.NumberOfTasks).ToList());
			return new CpuLog(logs.First().Date, usageProcessed, numberOfTasksProcessed);
		};
		
		Func<NpgsqlDataReader, CpuDbLog> parseRecordFunc = reader => _db.ParseCpuLogRecord(reader);
		var getEmptyRecordFunc = () => new CpuLog(DateTime.Now, 0, 0);

		var cpuOutput = new CpuOutput(serverId);
		await foreach (var log in QueryHelper.GetLogs(_db, "Cpu", sqlSelectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			cpuOutput.Logs.Add(log);
		}

		return cpuOutput;
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
	public async Task<MemoryOutput> Memory(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		string selectQuery = $"SELECT serverid, date, interval, mbused, mbtotal " +
		                     $"FROM Memory " +
		                     $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
		                     $"ORDER BY date";
	
		Func<IList<MemoryDbLog>, MemoryLog> combineLogsFunc = logs =>
		{
			int mbused = (int)QueryHelper.CombineValues(method, logs.Select(l => (double)l.MbUsed).ToList());
			int mbtotal = (int)QueryHelper.CombineValues(method, logs.Select(l => (double)l.MbTotal).ToList());
			return new MemoryLog(logs.First().Date, mbused, mbtotal);
		};
		
		Func<NpgsqlDataReader, MemoryDbLog> parseRecordFunc = reader => _db.ParseMemoryLogRecord(reader);
		var getEmptyRecordFunc = () => new MemoryLog(DateTime.Now, 0, 0);

		var memoryOutput = new MemoryOutput(serverId);
		await foreach(var log in QueryHelper.GetLogs(_db, "Memory", selectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			memoryOutput.Logs.Add(log);
		}

		return memoryOutput;
	}
}