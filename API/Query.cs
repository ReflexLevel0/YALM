using API.Models;

namespace API;

public class Query
{
	private readonly Db _db;

	public Query(Db db)
	{
		_db = db;
	}

	/// <summary>
	/// Returns cpu logs
	/// </summary>
	/// <param name="serverId"></param>
	/// <param name="startDateTime"></param>
	/// <param name="endDateTime"></param>
	/// <param name="interval">Interval that specifies time distance between two logs (for example, if interval is 10, then two logs of 5 minutes will be combined into a single log and returned)</param>
	/// <param name="method">Method for combining multiple logs into one (min, max, avg, etc.)</param>
	/// <returns></returns>
	public async IAsyncEnumerable<CpuLog> Cpu(int serverId, string? startDateTime, string? endDateTime, int interval, string method)
	{
		var cpuLogs = new List<CpuLog>();
		int intervalSum = 0;

		//Combines multiple cpu logs into a single log
		CpuLog CombineCpuLogs()
		{
			double usageProcessed = QueryHelper.CombineValues(method, cpuLogs.Select(c => c.Usage));
			int numberOfTasksProcessed = (int)QueryHelper.CombineValues(method, cpuLogs.Select(c => (double)c.NumberOfTasks));
			return new CpuLog(serverId, cpuLogs.First().Date, cpuLogs.Sum(c => c.Interval), usageProcessed, numberOfTasksProcessed);
		}

		await foreach (var reader in _db.ExecuteReadAsync(
			               $"SELECT serverid, date, interval, usage, numberoftasks " +
			               $"FROM Cpu " +
			               $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
			               $"ORDER BY date;"))
		{
			//Parsing db record data
			int id = reader.GetInt32(0);
			var date = reader.GetDateTime(1);
			int logInterval = reader.GetInt32(2);
			double usage = reader.GetDouble(3);
			int numberOfTasks = reader.GetInt32(4);
			var cpuLog = new CpuLog(id, date, logInterval, usage, numberOfTasks);
			
			//Adding cpu log to the list of logs
			cpuLogs.Add(cpuLog);
			intervalSum += cpuLog.Interval;
			if (intervalSum < interval) continue;
			
			//Returning the combined log
			yield return CombineCpuLogs();
			cpuLogs.Clear();
			intervalSum = 0;
		}

		if (cpuLogs.Count > 0) yield return CombineCpuLogs();
	}

	public RamLog Ram()
	{
		return new RamLog(0, DateTime.Today, 5, 1512, 8192);
	}
}