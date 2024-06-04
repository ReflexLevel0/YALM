using YALM.Common.Models;

namespace YALM.API.Db;

public class DatasetHelper<TDbLog, TLog>(Func<IList<TDbLog>, TLog> combineLogsFunc, Func<TLog> getEmptyLogFunc)
	where TDbLog : ILog
	where TLog : LogBase
{
	private DateTimeOffset? _nextDate;
	private readonly List<TDbLog> _logs = new();
	private double _intervalSum;
	private bool _break;

	/// <summary>
	/// Adds a log to the list of logs so that it can later on be combined into a singular log
	/// </summary>
	/// <param name="log"></param>
	/// <param name="interval"></param>
	/// <returns></returns>
	public IEnumerable<TLog?> AddLog(TDbLog log, double? interval)
	{
		//Combining logs and adding additional logs if there has been some kind of a break
		//between the last log and the current one so that the charts goes to 0 in case of break
		if (_nextDate != null && log.Date != _nextDate)
		{
			if (_logs.Count != 0) yield return CombineLogs();
			var emptyLog = getEmptyLogFunc();
			emptyLog.Date = _nextDate.Value;
			yield return emptyLog;
			_break = true;
		}

		//Returning a log if a break happened
		if (_break)
		{
			var emptyLog = getEmptyLogFunc();
			emptyLog.Date = log.Date.AddSeconds(-1);
			yield return emptyLog;
			_break = false;
		}
		
		//Adding log to the list of logs
		_logs.Add(log);
		_nextDate = log.Date.AddMinutes(log.Interval);
		_intervalSum += log.Interval;
		if (_intervalSum < interval) yield break;

		//Returning the combined log
		yield return CombineLogs();
	}
	
	/// <summary>
	/// Combines multiple logs into one
	/// </summary>
	/// <returns></returns>
	public TLog? CombineLogs()
	{
		if (_logs.Count == 0) return null;
		var log = combineLogsFunc(_logs);
		_logs.Clear();
		_intervalSum = 0;
		return log;
	}
}