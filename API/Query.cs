using API.Models.Graphql;
using API.Models.Logs;

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
	/// <param name="interval">Interval that specifies time distance between two logs (for example, if interval is 10, then two logs of 5 minutes will be combined into a single log and returned). If interval is null, then interval is decided dynamically (interval=1 minute for every hour between <param name="startDateTime"></param> and <param name="endDateTime"></param>).</param>
	/// <param name="method">Method for combining multiple logs into one (min, max, avg, etc.)</param>
	/// <returns></returns>
	public async IAsyncEnumerable<Cpu> Cpu(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		DateTime? lastDate = null;
		var cpuLogs = new List<CpuLog>();
		int intervalSum = 0;

		//Dynamically calculating the interval if it is null
		if (interval == null)
		{
			bool hasStart = DateTime.TryParse(startDateTime, out var startDate);
			bool hasEnd = DateTime.TryParse(endDateTime, out var endDate);
			if (hasStart == false)
			{
				await foreach (var reader in _db.ExecuteReadAsync("SELECT date FROM Cpu ORDER BY date LIMIT 1"))
				{
					startDate = reader.GetDateTime(0);
				}
			}
			if (hasEnd == false)
			{
				await foreach (var reader in _db.ExecuteReadAsync("SELECT date FROM Cpu ORDER BY date DESC LIMIT 1"))
				{
					endDate = reader.GetDateTime(0);
				}
			}

			interval = (int)endDate.Subtract(startDate).TotalHours;
		}
		
		//Combines multiple cpu logs into a single log
		Cpu CombineCpuLogs()
		{
			double usageProcessed = QueryHelper.CombineValues(method, cpuLogs.Select(c => c.Usage));
			int numberOfTasksProcessed = (int)QueryHelper.CombineValues(method, cpuLogs.Select(c => (double)c.NumberOfTasks));
			var log = new Cpu(serverId, cpuLogs.First().Date, usageProcessed, numberOfTasksProcessed);
			cpuLogs.Clear();
			intervalSum = 0;
			return log;
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

			//Combining logs and adding additional logs if there has been some kind of a break
			//between the last log and the current one so that the charts goes to 0 in case of break
			if (lastDate != null && date != lastDate)
			{
				var breakDate = cpuLogs.First().Date.AddMinutes(intervalSum);
				yield return CombineCpuLogs();
				yield return new Cpu(serverId, breakDate, 0, 0);
				yield return new Cpu(serverId, cpuLog.Date.Subtract(new TimeSpan(0, 0, 0, 1)), 0, 0);
			}
			
			//Adding cpu log to the list of logs
			cpuLogs.Add(cpuLog);
			lastDate = cpuLog.Date.AddMinutes(cpuLog.Interval);
			intervalSum += cpuLog.Interval;
			if (intervalSum < interval) continue;
			
			//Returning the combined log
			yield return CombineCpuLogs();
		}

		if (cpuLogs.Count > 0) yield return CombineCpuLogs();
	}

	public RamLog Ram()
	{
		return new RamLog(0, DateTime.Today, 5, 1512, 8192);
	}
}