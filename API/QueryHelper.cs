using System.Text;
using Common.Models;
using Common.Models.Graphql;
using Npgsql;

namespace API;

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

		return result.ToString();
	}

	public static double CombineValues<T>(string? method, IList<T> values)
	{
		bool longMode = values.All(v => v is long);
		double result;
		switch (method)
		{
			case null:
			case "avg":
			case "average":
				result = longMode ? values.Cast<long>().Average() : values.Cast<double>().Average();
				break;
			case "min":
				result = longMode ? values.Cast<long>().Min() : values.Cast<double>().Min();
				break;
			case "max":
				result = longMode ? values.Cast<long>().Max() : values.Cast<double>().Max();
				break;
			default:
				throw new Exception($"Invalid method '{method}'");
		}

		return result;
	}

	/// <summary>
	/// Dynamically calculates the interval based on start and end date so that the API doesn't return too much data
	/// </summary>
	/// <param name="db">Database</param>
	/// <param name="startDateString">Starting date for interval calculation</param>
	/// <param name="endDateString">Ending date for interval calculation</param>
	/// <param name="tableName">SQL table from which data is pulled</param>
	/// <returns></returns>
	public static async Task<int> CalculateInterval(IDb db, string? startDateString, string? endDateString, string tableName)
	{
		bool hasStart = DateTime.TryParse(startDateString, out var startDate);
		bool hasEnd = DateTime.TryParse(endDateString, out var endDate);
		
		//Getting first log date
		string startQuery = $"SELECT date FROM {tableName} " +
		                    $"{(hasStart ? $"WHERE date>='{startDateString}'" : "")} " +
		                    $"ORDER BY date LIMIT 1";
		await foreach (var reader in db.ExecuteReaderAsync(startQuery))
		{
			startDate = reader.GetDateTime(0);
		}
		
		//Getting last log date
		string endQuery = $"SELECT date FROM {tableName} " +
		                  $"{(hasEnd ? $"WHERE date<='{endDateString}'" : "")} " +
		                  $"ORDER BY date DESC LIMIT 1";
		await foreach (var reader in db.ExecuteReaderAsync(endQuery))
		{
			endDate = reader.GetDateTime(0);
		}

		//Interval is calculated based on the range between first and last log
		return (int)endDate.Subtract(startDate).TotalHours;
	}

	/// <summary>
	/// Returns a generic list of logs
	/// </summary>
	/// <param name="db">Database from which data is pulled</param>
	/// <param name="tableName">Table name in the database where data is located</param>
	/// <param name="sqlSelectQuery">SQL query which returns logs from the database</param>
	/// <param name="combineLogsFunc">Function to combine multiple logs into one</param>
	/// <param name="parseRecordFunc">Function used to parse a database record and return a log</param>
	/// <param name="getEmptyLogFunc">Function that returns an empty log</param>
	/// <param name="startDateTime">Start date for the data (if null, start is unlimited)</param>
	/// <param name="endDateTime">End date for the data (if null, end is unlimited)</param>
	/// <param name="interval">Interval between different log points (multiple points withing the interval are combined into one; if null then it is calculated dynamically)</param>
	/// <typeparam name="TLog">Type of logs returned from the database</typeparam>
	/// <typeparam name="TGraphqlLog">Types of the logs returned to the user by the API</typeparam>
	/// <returns></returns>
	public static async IAsyncEnumerable<TGraphqlLog> GetLogs<TLog, TGraphqlLog>(
		IDb db,
		string tableName,
		string sqlSelectQuery,
		Func<IList<TLog>, TGraphqlLog> combineLogsFunc,
		Func<NpgsqlDataReader, TLog> parseRecordFunc,
		Func<TGraphqlLog> getEmptyLogFunc,
		string? startDateTime,
		string? endDateTime,
		int? interval) where TLog : IDbLogBase where TGraphqlLog : GraphqlModelBase
	{
		DateTime? lastDate = null;
		DateTime? breakDate = null;
		var logs = new List<TLog>();
		int intervalSum = 0;
		
		//Dynamically calculating the interval
		interval ??= await CalculateInterval(db, startDateTime, endDateTime, tableName);
		
		//Combines multiple logs into a single log
		TGraphqlLog CombineLogs()
		{
			var log = combineLogsFunc(logs);
			logs.Clear();
			intervalSum = 0;
			return log;
		}
		
		//Going through each record in the database
		await foreach (var reader in db.ExecuteReaderAsync(sqlSelectQuery))
		{
			var log = parseRecordFunc(reader);

			//Combining logs and adding additional logs if there has been some kind of a break
			//between the last log and the current one so that the charts goes to 0 in case of break
			if (lastDate != null && breakDate != null && log.Date != lastDate)
			{
				if (logs.Count != 0) yield return CombineLogs();
				var emptyLog = getEmptyLogFunc();
				emptyLog.Date = (DateTime)breakDate;
				yield return emptyLog;
				emptyLog.Date = emptyLog.Date.Subtract(new TimeSpan(0, 0, 0, 1));
				yield return emptyLog;
			}

			//Adding log to the list of logs
			logs.Add(log);
			breakDate = log.Date.AddSeconds(1);
			lastDate = log.Date.AddMinutes(log.Interval);
			intervalSum += log.Interval;
			if (intervalSum < interval) continue;

			//Returning the combined log
			yield return CombineLogs();
		}

		//Combining last of the logs into a single log and returning it
		if (logs.Count > 0) yield return CombineLogs();
	} 
}