using Common.Models;
using Common.Models.Graphql;

namespace API;

public class DatasetHelper<TLog, TGraphqlLog> where TLog : IDbLogBase where TGraphqlLog : GraphqlModelBase
{
	public string Hash { get; }
	private DateTime? _nextDate;
	private readonly List<TLog> _logs = new();
	private int _intervalSum;
	private bool _break;
	private readonly Func<IList<TLog>, TGraphqlLog> _combineLogsFunc;
	private readonly Func<TGraphqlLog> _getEmptyLogFunc;
	
	public DatasetHelper(Func<IList<TLog>, TGraphqlLog> combineLogsFunc, Func<TGraphqlLog> getEmptyLogFunc, string hash)
	{
		_combineLogsFunc = combineLogsFunc;
		_getEmptyLogFunc = getEmptyLogFunc;
		Hash = hash;
	}
	
	/// <summary>
	/// Adds a log to the list of logs so that it can later on be combined into a singular log
	/// </summary>
	/// <param name="log"></param>
	/// <param name="interval"></param>
	/// <returns></returns>
	public IEnumerable<TGraphqlLog?> AddLog(TLog log, int? interval)
	{
		//Combining logs and adding additional logs if there has been some kind of a break
		//between the last log and the current one so that the charts goes to 0 in case of break
		if (_nextDate != null && log.Date != _nextDate)
		{
			if (_logs.Count != 0) yield return CombineLogs();
			var emptyLog = _getEmptyLogFunc();
			emptyLog.Date = _nextDate.Value;
			yield return emptyLog;
			_break = true;
		}

		//Returning a log if a break happened
		if (_break)
		{
			var emptyLog = _getEmptyLogFunc();
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
	public TGraphqlLog? CombineLogs()
	{
		if (_logs.Count == 0) return null;
		var log = _combineLogsFunc(_logs);
		_logs.Clear();
		_intervalSum = 0;
		return log;
	}
}