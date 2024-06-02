using System.Text;
using LinqToDB;
using YALM.API.Db;
using YALM.Common.Models;

namespace YALM.API;

public class QueryHelper
{
	public static string LimitSqlByParameters(int? serverId, string? startDateTime, string? endDateTime)
	{
		var result = new StringBuilder(256);

		if (serverId != null) result.Append($"serverId = {serverId}");

		if (string.IsNullOrWhiteSpace(startDateTime) == false)
		{
			if (result.Length != 0) result.Append(" AND ");
			result.Append($"date >= '{startDateTime}'");
		}

		if (string.IsNullOrWhiteSpace(endDateTime) == false)
		{
			if (result.Length != 0) result.Append(" AND ");
			result.Append($"date <= '{endDateTime}'");
		}

		//Making where always true if all parameters are null
		if (string.IsNullOrWhiteSpace(result.ToString())) result.Append("1 = 1");

		return result.ToString();
	}
	
	public static decimal? CombineValues<T>(string? method, IEnumerable<T> values)
	{
		var filteredValues = values.Where(v => v != null);
		var decimalValues = filteredValues.Select(v => Convert.ToDecimal(v)).ToList();
		if (decimalValues.Count == 0) return null;
		decimal result;
		switch (method)
		{
			case null:
			case "avg":
			case "average":
				result = decimalValues.Average();
				break;
			case "min":
				result = decimalValues.Min();
				break;
			case "max":
				result = decimalValues.Max();
				break;
			default:
				throw new Exception($"Invalid method '{method}'");
		}

		return result;
	}

	/// <summary>
	/// Dynamically calculates the interval based on start and end date so that the API doesn't return too much data
	/// </summary>
	/// <param name="table">Table from which data is fetched</param>
	/// <param name="startDate">Starting date for interval calculation</param>
	/// <param name="endDate">Ending date for interval calculation</param>
	/// <returns></returns>
	public static async Task<double> CalculateInterval(IQueryable<ILog> table, DateTimeOffset? startDate, DateTimeOffset? endDate)
	{
		var sd = startDate ?? DateTimeOffset.MinValue;
		var startDateQuery = 
			from l in table 
			where l.Date >= sd 
			orderby l.Date select l.Date;
		startDate = await startDateQuery.FirstOrDefaultAsync();

		var ed = endDate ?? DateTimeOffset.MaxValue;
		var endDateQuery =
			from l in table
			where l.Date <= ed
			orderby l.Date descending select l.Date;
		endDate = await endDateQuery.FirstOrDefaultAsync();

		double interval = (int)((DateTimeOffset)endDate).Subtract((DateTimeOffset)startDate).TotalHours + 1;
		return interval;
	}

	/// <summary>
	/// Returns a generic list of logs
	/// </summary>
	/// <param name="table">Table from which the logs are fetched</param>
	/// <param name="combineLogsFunc">Function to combine multiple logs into one</param>
	/// <param name="getEmptyLogFunc">Function that returns an empty log</param>
	/// <param name="calculateHash">Used for calculating hash of a log (this is important when there are multiple sets of data returned, for example 10 logs at the same date-time for 10 different cpu cores, so that dates and breaks between them don't get mixed up</param>
	/// <param name="startDateTime">Start date for the data (if null, start is unlimited)</param>
	/// <param name="endDateTime">End date for the data (if null, end is unlimited)</param>
	/// <param name="interval">Interval between different log points (multiple points withing the interval are combined into one; if null then it is calculated dynamically)</param>
	/// <typeparam name="TDbLog">Type of logs returned from the database</typeparam>
	/// <typeparam name="TLog">Types of the logs returned to the user by the API</typeparam>
	/// <returns></returns>
	public static async IAsyncEnumerable<TLog> GetLogs<TDbLog, TLog>(
		IQueryable<ILog> table,
		Func<IList<TDbLog>, TLog> combineLogsFunc,
		Func<TLog> getEmptyLogFunc,
		Func<TDbLog, string> calculateHash,
		DateTimeOffset? startDateTime,
		DateTimeOffset? endDateTime,
		double? interval) where TDbLog : ILog where TLog : LogBase
	{
		var limits = new List<DatasetHelper<TDbLog, TLog>>();

		//Dynamically calculating the interval
		interval ??= await CalculateInterval(table, startDateTime, endDateTime);

		//Going through each record in the database
		var selectQuery = from l in table select l;
		if (startDateTime != null) selectQuery = selectQuery.Where(l => l.Date >= startDateTime.Value.ToLocalTime());
		if (endDateTime != null) selectQuery = selectQuery.Where(l => l.Date <= endDateTime.Value.ToLocalTime());
		foreach (var log in selectQuery)
		{
			string hash = calculateHash((TDbLog)log);
			var limit = limits.FirstOrDefault(l => string.Compare(l.Hash, hash, StringComparison.Ordinal) == 0);
			if (limit == null)
			{
				limit = new DatasetHelper<TDbLog, TLog>(combineLogsFunc, getEmptyLogFunc, hash);
				limits.Add(limit);
			}

			//Adding the log to the list of logs
			foreach (var point in limit.AddLog((TDbLog)log, interval))
			{
				if (point == null) continue;
				yield return point;
			}
		}

		//Combining last of the logs into a single log and returning it
		foreach (var limit in limits)
		{
			var point = limit.CombineLogs();
			if (point != null) yield return point;
		}
	}
}