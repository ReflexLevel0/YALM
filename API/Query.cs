using YALM.API.Models.Db;
using YALM.Common.Models.Graphql.Logs;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API;

public class Query(IDb db)
{
	/// <summary>
	/// Returns cpu logs
	/// </summary>
	/// <param name="serverId"></param>
	/// <param name="startDateTime"></param>
	/// <param name="endDateTime"></param>
	/// <param name="interval">Interval that specifies time distance between two logs. If interval is null, then interval is decided dynamically (interval=1 minute for every hour between <param name="startDateTime"></param> and <param name="endDateTime"></param>).</param>
	/// <param name="method">Method for combining multiple logs into one (min, max, avg, etc.)</param>
	/// <returns></returns>
	public async Task<CpuOutput> Cpu(int serverId, DateTime? startDateTime, DateTime? endDateTime, int? interval, string? method)
	{
		Func<IList<DataModel.CpuLogDbRecord>, CpuLog> combineLogsFunc = logs =>
		{
			double usageProcessed = QueryHelper.CombineValues(method, logs.Select(c => c.Usage).ToList());
			var numberOfTasksValues = logs.Where(c => c.NumberOfTasks != null).Select(c => c.NumberOfTasks!);
			int numberOfTasks = (int)QueryHelper.CombineValues(method, numberOfTasksValues.ToList());
			return new CpuLog { Date = logs.First().Date, Usage = usageProcessed, NumberOfTasks = numberOfTasks};
		};
		
		var getEmptyRecordFunc = () => new CpuLog();

		var cpuOutput = new CpuOutput(serverId);
		await foreach (var log in QueryHelper.GetLogs(db.CpuLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			cpuOutput.Logs.Add(log);
		}

		return cpuOutput;
	}

	// /// <summary>
	// /// Returns memory logs
	// /// </summary>
	// /// <param name="serverId"></param>
	// /// <param name="startDateTime"></param>
	// /// <param name="endDateTime"></param>
	// /// <param name="interval">Interval that specifies time distance between two logs. If interval is null, then interval is decided dynamically (interval=1 minute for every hour between <param name="startDateTime"></param> and <param name="endDateTime"></param>).</param>
	// /// <param name="method">Method for combining multiple logs into one (min, max, avg, etc.)</param>
	// /// <returns></returns>
	// public async Task<MemoryOutput> Memory(int serverId, string? startDateTime, string? endDateTime, int? interval, string? method)
	// {
	// 	string selectQuery = $"SELECT serverid, date, interval, mbused, mbtotal " +
	// 	                     $"FROM Memory " +
	// 	                     $"WHERE {QueryHelper.LimitSqlByParameters(serverId, startDateTime, endDateTime)} " +
	// 	                     $"ORDER BY date";
	//
	// 	Func<IList<MemoryDbLog>, MemoryLog> combineLogsFunc = logs =>
	// 	{
	// 		int mbused = (int)QueryHelper.CombineValues(method, logs.Select(l => (double)l.MbUsed).ToList());
	// 		int mbtotal = (int)QueryHelper.CombineValues(method, logs.Select(l => (double)l.MbTotal).ToList());
	// 		return new MemoryLog(logs.First().Date, mbused, mbtotal);
	// 	};
	// 	
	// 	Func<NpgsqlDataReader, MemoryDbLog> parseRecordFunc = reader => db.ParseMemoryLogRecord(reader);
	// 	var getEmptyRecordFunc = () => new MemoryLog(DateTime.Now, 0, 0);
	//
	// 	var memoryOutput = new MemoryOutput(serverId);
	// 	await foreach(var log in QueryHelper.GetLogs(db, "Memory", selectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
	// 	{
	// 		memoryOutput.Logs.Add(log);
	// 	}
	//
	// 	return memoryOutput;
	// }
	//
	// /// <summary>
	// /// Returns disk data
	// /// </summary>
	// public async IAsyncEnumerable<DiskOutput> Disk(string? startDateTime, string? endDateTime, int? interval, string? method)
	// {
	// 	DiskOutput? disk = null;
	// 	string getPartitionQuery = "SELECT disk.serverid, disk.label as diskLabel, uuid, filesystemname, filesystemversion, partition.label as partitionLabel, mountpath " +
	// 	                           "FROM disk JOIN partition ON disk.id = partition.diskid " + 
	// 	                           "ORDER BY diskid";
	// 	
	// 	//Going through every partition and getting logs for it
	// 	await foreach (var reader in db.ExecuteReaderAsync(getPartitionQuery))
	// 	{
	// 		int id = reader.GetInt32(0);
	// 		string diskLabel = reader.GetString(1);
	// 		string uuid = reader.GetString(2);
	// 		string filesystemName = reader.GetString(3);
	// 		string filesystemVersion = reader.GetString(4);
	// 		string partitionLabel = reader.GetString(5);
	// 		string mountPath = reader.GetString(6);
	// 		if (disk == null || string.CompareOrdinal(disk.Label, diskLabel) != 0)
	// 		{
	// 			if(disk != null) yield return disk;
	// 			disk = new DiskOutput(id, diskLabel);
	// 		}
	//
	// 		var partition = new PartitionOutput(uuid, filesystemName, filesystemVersion, partitionLabel, mountPath);
	// 		disk.Partitions.Add(partition);
	// 		
	// 		//Getting logs for the partition
	// 		string selectQuery = "SELECT diskid, uuid, date, interval, bytestotal, usage " + 
	// 		                     "FROM partitionlog " +
	// 		                     $"WHERE {QueryHelper.LimitSqlByParameters(null, startDateTime, endDateTime)} " +
	// 		                     "ORDER BY date";
	// 		
	// 		Func<IList<PartitionDbLog>, PartitionLog> combineLogsFunc = logs =>
	// 		{
	// 			long bytes = (int)QueryHelper.CombineValues(method, logs.Select(l => l.Bytes).ToList());
	// 			int usage = (int)QueryHelper.CombineValues(method, logs.Select(l => l.UsedPercentage).ToList());
	// 			return new PartitionLog(logs.First().Date, bytes, usage);
	// 		};
	// 	
	// 		Func<NpgsqlDataReader, PartitionDbLog> parseRecordFunc = r => db.ParsePartitionLogRecord(r);
	// 		var getEmptyRecordFunc = () => new PartitionLog(DateTime.Now, 0, 0);
	// 		
	// 		await foreach(var log in QueryHelper.GetLogs(db, "PartitionLog", selectQuery, combineLogsFunc, parseRecordFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
	// 		{
	// 			partition.Logs.Add(log);
	// 		}
	// 	}
	//
	// 	if (disk != null) yield return disk;
	// }
}