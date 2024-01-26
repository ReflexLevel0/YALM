using DataModel;
using LinqToDB;
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
		var getEmptyRecordFunc = () => new CpuLog();
		Func<IList<CpuLogDbRecord>, CpuLog> combineLogsFunc = logs =>
		{
			double usageProcessed = (double)QueryHelper.CombineValues(method, logs.Select(c => c.Usage).ToList());
			var numberOfTasksValues = logs.Where(c => c.NumberOfTasks != null).Select(c => c.NumberOfTasks!);
			int numberOfTasks = (int)QueryHelper.CombineValues(method, numberOfTasksValues.ToList());
			return new CpuLog { Date = logs.First().Date.ToLocalTime(), Usage = usageProcessed, NumberOfTasks = numberOfTasks };
		};
		
		var cpuOutput = new CpuOutput(serverId);
		var cpu = await (from c in db.Cpus where c.ServerId == serverId select c).FirstOrDefaultAsync();
		if (cpu != null)
		{
			cpuOutput.Name = cpu.Name;
			cpuOutput.Architecture = cpu.Architecture;
			cpuOutput.Cores = cpu.Cores;
			cpuOutput.Threads = cpu.Threads;
			cpuOutput.Frequency = cpu.FrequencyMhz;
		} 
		await foreach (var log in QueryHelper.GetLogs(db.CpuLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			cpuOutput.Logs.Add(log);
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
	public async Task<MemoryOutput> Memory(int serverId, DateTime? startDateTime, DateTime? endDateTime, int? interval, string? method)
	{
		var getEmptyRecordFunc = () => new MemoryLog { Date = DateTime.Now };
		Func<IList<MemoryLogDbRecord>, MemoryLog> combineLogsFunc = logs =>
		{
			long totalKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.TotalKb));
			long freeKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.FreeKb));
			long usedKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.UsedKb));
			long swapTotalKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.SwapTotalKb));
			long swapFreeKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.SwapFreeKb));
			long swapUsedKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.SwapUsedKb));
			long cachedKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.CachedKb));
			long availableKb = (long)QueryHelper.CombineValues(method, logs.Select(l => l.AvailableKb));
			return new MemoryLog
			{
				Date = logs.First().Date.ToLocalTime(),
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
		await foreach (var log in QueryHelper.GetLogs(db.MemoryLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			memoryOutput.Logs.Add(log);
		}

		return memoryOutput;
	}

	/// <summary>
	/// Returns program logs
	/// </summary>
	public async Task<ProgramOutput> Program(int serverId, DateTime? startDateTime, DateTime? endDateTime, int? interval, string? method)
	{
		var getEmptyRecordFunc = () => new ProgramLog { Name = "", Date = DateTime.Now };
		Func<IList<ProgramLogDbRecord>, ProgramLog> combineLogsFunc = logs =>
		{
			decimal cpuUsage = QueryHelper.CombineValues(method, logs.Select(l => l.CpuutilizationPercentage));
			decimal memoryUsage = QueryHelper.CombineValues(method, logs.Select(l => l.MemoryUtilizationPercentage));
			return new ProgramLog
			{
				Date = logs.First().Date.ToLocalTime(),
				Name = logs.First().Name,
				CpuUsage = cpuUsage,
				MemoryUsage = memoryUsage
			};
		};

		var programOutput = new ProgramOutput(serverId);
		await foreach (var log in QueryHelper.GetLogs(db.ProgramLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			programOutput.Logs.Add(log);
		}

		return programOutput;
	}

	/// <summary>
	/// Returns disk data
	/// </summary>
	public async IAsyncEnumerable<DiskOutput> Disk(DateTime? startDateTime, DateTime? endDateTime, int? interval, string? method)
	{
		var getEmptyRecordFunc = () => new PartitionLog { Date = DateTime.Now };
		Func<IList<PartitionLogDbRecord>, PartitionLog> combineLogsFunc = logs =>
		{
			long bytes = (long)QueryHelper.CombineValues(method, logs.Select(l => l.BytesTotal).ToList());
			decimal usage = QueryHelper.CombineValues(method, logs.Select(l => l.Usage).ToList());
			return new PartitionLog { Date = logs.First().Date.ToLocalTime(), Bytes = bytes, UsedPercentage = usage };
		};

		//Going through every disk and getting data for it
		var disks = await db.Disks.ToListAsync();
		foreach (var d in disks)
		{
			var partitions = await 
				(from p in db.Partitions 
				where p.DiskId == d.Id
				select p).ToListAsync();

			var disk = new DiskOutput(d.ServerId, d.Label);
			foreach (var partition in partitions)
			{
				var partitionOutput = (PartitionOutput)Convert.ChangeType(partition, typeof(PartitionOutput));
				disk.Partitions.Add(partitionOutput);
				
				//Getting all logs for this partition
				await foreach(var log in QueryHelper.GetLogs(db.PartitionLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
				{
					partitionOutput.Logs.Add(log);
				}
			}

			yield return disk;
		}
	}
}