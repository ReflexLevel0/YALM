using API.Models;

namespace API;

public class Query
{
	private readonly Db _db;

	public Query(Db db)
	{
		_db = db;
	}

	public async IAsyncEnumerable<Cpu> Cpu(int serverId, string? startDateTime, string? endDateTime, int interval, string method)
	{
		var cpuLogs = new List<Cpu>();
		int intervalSum = 0;

		Cpu CombineCpuLogs()
		{
			double usageProcessed = QueryHelper.CombineValues(method, cpuLogs.Select(c => c.Usage));
			int numberOfTasksProcessed = (int)QueryHelper.CombineValues(method, cpuLogs.Select(c => (double)c.NumberOfTasks));
			return new Cpu(serverId, cpuLogs.First().Date, cpuLogs.Sum(c => c.Interval), usageProcessed, numberOfTasksProcessed);
		}

		await foreach (var reader in _db.ExecuteReadAsync(
			               $"SELECT serverid, date, interval, usage, numberoftasks " +
			               $"FROM Cpu WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} ORDER BY date;"))
		{
			int id = reader.GetInt32(0);
			var date = reader.GetDateTime(1);
			int logInterval = reader.GetInt32(2);
			double usage = reader.GetDouble(3);
			int numberOfTasks = reader.GetInt32(4);
			var cpuLog = new Cpu(id, date, logInterval, usage, numberOfTasks);
			
			cpuLogs.Add(cpuLog);
			intervalSum += cpuLog.Interval;
			if (intervalSum < interval) continue;
			
			yield return CombineCpuLogs();
			cpuLogs.Clear();
			intervalSum = 0;
		}

		if (cpuLogs.Count > 0) yield return CombineCpuLogs();
	}

	public Ram Ram()
	{
		return new Ram(0, DateTime.Today, 5, 1512, 8192);
	}
}