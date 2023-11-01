using API.Models.Db;
using Common.Models.Graphql;

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
	/// <param name="interval">Interval that specifies time distance between two logs (for example, if interval is 10, then two logs of 5 minutes will be combined into a single log and returned). If interval is null, then interval is decided dynamically (interval=1 minute for every hour between <param name="startDateTime"></param> and <param name="endDateTime"></param>).</param>
	/// <param name="method">Method for combining multiple logs into one (min, max, avg, etc.)</param>
	/// <returns></returns>
	public async IAsyncEnumerable<Cpu> Cpu(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	{
		DateTime? lastDate = null;
		DateTime? breakDate = null;
		var cpuLogs = new List<CpuLog>();
		int intervalSum = 0;

		//Dynamically calculating the interval if it is null
		interval ??= await CalculateInterval(startDateTime, endDateTime, "CPU");

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

		await foreach (var reader in _db.ExecuteReaderAsync(
			               $"SELECT serverid, date, interval, usage, numberoftasks " +
			               $"FROM Cpu " +
			               $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
			               $"ORDER BY date;"))
		{
			var cpuLog = _db.ParseCpuRecord(reader);

			//Combining logs and adding additional logs if there has been some kind of a break
			//between the last log and the current one so that the charts goes to 0 in case of break
			if (lastDate != null && breakDate != null && cpuLog.Date != lastDate)
			{
				if (cpuLogs.Count != 0) yield return CombineCpuLogs();
				yield return new Cpu(serverId, (DateTime)breakDate, 0, 0);
				yield return new Cpu(serverId, cpuLog.Date.Subtract(new TimeSpan(0, 0, 0, 1)), 0, 0);
			}

			//Adding cpu log to the list of logs
			cpuLogs.Add(cpuLog);
			breakDate = cpuLog.Date.AddSeconds(1);
			lastDate = cpuLog.Date.AddMinutes(cpuLog.Interval);
			intervalSum += cpuLog.Interval;
			if (intervalSum < interval) continue;

			//Returning the combined log
			yield return CombineCpuLogs();
		}

		if (cpuLogs.Count > 0) yield return CombineCpuLogs();
	}

	/// <summary>
	/// Dynamically calculates the interval based on start and end date so that the API doesn't return too much data
	/// </summary>
	/// <param name="startDateString"></param>
	/// <param name="endDateString"></param>
	/// <param name="tableName"></param>
	/// <returns></returns>
	public async Task<int> CalculateInterval(string? startDateString, string? endDateString, string tableName)
	{
		bool hasStart = DateTime.TryParse(startDateString, out var startDate);
		bool hasEnd = DateTime.TryParse(endDateString, out var endDate);
		
		//Getting first log date
		string startQuery = $"SELECT date FROM {tableName} " +
		                    $"{(hasStart ? $"WHERE date>='{startDateString}'" : "")} " +
		                    $"ORDER BY date LIMIT 1";
		await foreach (var reader in _db.ExecuteReaderAsync(startQuery))
		{
			startDate = reader.GetDateTime(0);
		}
		
		//Getting last log date
		string endQuery = $"SELECT date FROM {tableName} " +
		                  $"{(hasEnd ? $"WHERE date<='{endDateString}'" : "")} " +
		                  $"ORDER BY date DESC LIMIT 1";
		await foreach (var reader in _db.ExecuteReaderAsync(endQuery))
		{
			endDate = reader.GetDateTime(0);
		}

		//Interval is calculated based on the range between first and last log
		return (int)endDate.Subtract(startDate).TotalHours;
	}

	public RamLog Ram()
	{
		return new RamLog(0, DateTime.Today, 5, 1512, 8192);
	}
}