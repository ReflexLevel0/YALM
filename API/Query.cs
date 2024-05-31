using DataModel;
using LinqToDB;
using YALM.API.Alerts;
using YALM.API.Db;
using YALM.API.Db.Models;
using YALM.Common;
using YALM.Common.Models.Graphql.Logs;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API;

public class Query(IDbProvider dbProvider)
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
	public async Task<CpuOutput?> Cpu(int serverId, DateTime? startDateTime, DateTime? endDateTime, double? interval, string? method)
	{
		var getEmptyRecordFunc = () => new CpuLog();
		Func<IList<CpuLogDbRecord>, CpuLog> combineLogsFunc = logs =>
		{
			double? usageProcessed = (double?)QueryHelper.CombineValues(method, logs.Select(c => c.Usage).ToList());
			var numberOfTasksValues = logs.Where(c => c.NumberOfTasks != null).Select(c => c.NumberOfTasks!);
			int? numberOfTasks = (int?)QueryHelper.CombineValues(method, numberOfTasksValues.ToList());
			return new CpuLog { Date = logs.First().Date, Usage = usageProcessed, NumberOfTasks = numberOfTasks };
		};
		
		var cpuOutput = new CpuOutput(serverId);
		await using (var db = dbProvider.GetDb())
		{
			var cpu = await (from c in db.Cpus where c.ServerId == serverId select c).FirstOrDefaultAsync();
			if (cpu == null) return null;
		
			cpuOutput.Name = cpu.Name;
			cpuOutput.Architecture = cpu.Architecture;
			cpuOutput.Cores = cpu.Cores;
			cpuOutput.Threads = cpu.Threads;
			cpuOutput.Frequency = cpu.FrequencyMhz;
			await foreach (var log in QueryHelper.GetLogs(db.CpuLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
			{
				cpuOutput.Logs.Add(log);
			}
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
	public async Task<MemoryOutput> Memory(int serverId, DateTime? startDateTime, DateTime? endDateTime, double? interval, string? method)
	{
		var getEmptyRecordFunc = () => new MemoryLog { Date = DateTime.Now };
		Func<IList<MemoryLogDbRecord>, MemoryLog> combineLogsFunc = logs =>
		{
			long? totalKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.TotalKb));
			long? freeKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.FreeKb));
			long? usedKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.UsedKb));
			long? swapTotalKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.SwapTotalKb));
			long? swapFreeKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.SwapFreeKb));
			long? swapUsedKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.SwapUsedKb));
			long? cachedKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.CachedKb));
			long? availableKb = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.AvailableKb));
			return new MemoryLog
			{
				Date = logs.First().Date,
				TotalKb = totalKb,
				FreeKb = freeKb,
				UsedKb = usedKb,
				SwapTotalKb = swapTotalKb,
				SwapFreeKb = swapFreeKb,
				SwapUsedKb = swapUsedKb, 
				CachedKb = cachedKb,
				AvailableKb = availableKb
			};
		};
		
		var memoryOutput = new MemoryOutput(serverId);
		await using (var db = dbProvider.GetDb())
		{
			await foreach (var log in QueryHelper.GetLogs(db.MemoryLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
			{
				memoryOutput.Logs.Add(log);
			}
		}

		return memoryOutput;
	}

	/// <summary>
	/// Returns program logs
	/// </summary>
	public async Task<ProgramOutput> Program(int serverId, DateTime? startDateTime, DateTime? endDateTime, double? interval, string? method)
	{
		var getEmptyRecordFunc = () => new ProgramLog { Name = "", Date = DateTime.Now };
		Func<IList<ProgramLogDbRecord>, ProgramLog> combineLogsFunc = logs =>
		{
			decimal? cpuUsage = QueryHelper.CombineValues(method, logs.Select(l => l.CpuutilizationPercentage));
			decimal? memoryUsage = QueryHelper.CombineValues(method, logs.Select(l => l.MemoryUtilizationPercentage));
			return new ProgramLog
			{
				Date = logs.First().Date,
				Name = logs.First().Name,
				CpuUsage = cpuUsage,
				MemoryUsage = memoryUsage
			};
		};

		var programOutput = new ProgramOutput(serverId);

		await using (var db = dbProvider.GetDb())
		{
			await foreach (var log in QueryHelper.GetLogs(db.ProgramLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
			{
				programOutput.Logs.Add(log);
			}
		}

		return programOutput;
	}

	// /// <summary>
	// /// Returns disk data
	// /// </summary>
	public async IAsyncEnumerable<DiskOutput> Disk(int serverId, string? uuid, DateTime? startDateTime, DateTime? endDateTime, double? interval, string? method)
	{
		var getEmptyRecordFunc = () => new PartitionLog { Date = DateTime.Now };
		Func<IList<PartitionLogDbRecord>, PartitionLog> combineLogsFunc = logs =>
		{
			long? bytes = (long?)QueryHelper.CombineValues(method, logs.Select(l => l.BytesTotal).ToList());
			decimal? usage = QueryHelper.CombineValues(method, logs.Select(l => l.Usage).ToList());
			return new PartitionLog { Date = logs.First().Date, Bytes = bytes, UsedPercentage = usage };
		};
	
		//Going through every disk and getting data for it
		await using (var db = dbProvider.GetDb())
		{
			List<DiskDbRecord> disks = await 
				(from disk in db.Disks 
					where disk.ServerId == serverId && (uuid == null || string.CompareOrdinal(disk.Uuid, uuid) == 0)
					select disk).ToListAsync();
			foreach (var d in disks)
			{
				var partitions = await 
					(from p in db.Partitions 
						where p.Diskuuid == d.Uuid && p.Serverid == d.ServerId
						select p).ToListAsync();
	
				var disk = new DiskOutput(d.ServerId, d.Uuid, d.Type, d.Serial, d.Path, d.Vendor, d.Model, d.BytesTotal);
				foreach (var partition in partitions)
				{
					var partitionOutput = (PartitionOutput)Convert.ChangeType(partition, typeof(PartitionOutput));
					disk.Partitions.Add(partitionOutput);
				
					//Getting all logs for this partition
					var partitionLogs = 
						from l in db.PartitionLogs
						where l.Serverid == partition.Serverid && l.Partitionuuid == partition.Uuid
						select l;
					await foreach(var log in QueryHelper.GetLogs(partitionLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
					{
						partitionOutput.Logs.Add(log);
					}
				}
	
				yield return disk;
			}
		}
	}
	
	// /// <summary>
	// /// Returns alerts
	// /// </summary>
	public async IAsyncEnumerable<AlertOutput> Alert(int? serverId, DateTime? startDateTime, DateTime? endDateTime)
	{
		await using var db = dbProvider.GetDb();
		var alerts = await (from alert in db.Alerts
			where alert.Serverid == (serverId ?? alert.Serverid) && alert.Date >= (startDateTime ?? DateTime.MinValue) && alert.Date <= (endDateTime ?? DateTime.MaxValue)
			orderby alert.Date, alert.Serverid
			select alert).ToListAsync();
		foreach (var alert in alerts)
		{
			yield return new AlertOutput(alert.Serverid, alert.Date, (AlertSeverity)alert.Severity, alert.Text);
		}
	}

	/// <summary>
	/// Returns server status
	/// </summary>
	public async IAsyncEnumerable<ServerOutput> Server(int? serverId)
	{
		await using var db = dbProvider.GetDb();
		var statusList = await (from s in db.ServerStatus where s.Serverid == (serverId ?? s.Serverid) select s).ToListAsync();
		foreach (var status in statusList)
		{
			Console.WriteLine($"{status.Serverid} {status.Status}");
			yield return new ServerOutput{ServerId = status.Serverid, Online = string.CompareOrdinal(status.Status, "online") == 0};
		}
	}
}