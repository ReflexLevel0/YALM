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
			return new CpuLog { Date = logs.First().Date, Usage = usageProcessed, NumberOfTasks = numberOfTasks };
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
			decimal usedPercentage = QueryHelper.CombineValues(method, logs.Where(l => l.UsedPercentage != null).Select(l => (decimal)l.UsedPercentage!));
			int usedKb = (int)QueryHelper.CombineValues(method, logs.Where(l => l.UsedKb != null).Select(l => (int)l.UsedKb!));
			int swapUsedKb = (int)QueryHelper.CombineValues(method, logs.Where(l => l.SwapUsedKb != null).Select(l => (int)l.SwapUsedKb!));
			int cachedKb = (int)QueryHelper.CombineValues(method, logs.Where(l => l.CachedKb != null).Select(l => (int)l.CachedKb!));
			return new MemoryLog { Date = logs.First().Date, UsedPercentage = usedPercentage, UsedKb = usedKb, SwapUsedKb = swapUsedKb, CachedKb = cachedKb };
		};

		var memoryOutput = new MemoryOutput(serverId);
		await foreach (var log in QueryHelper.GetLogs(db.MemoryLogs, combineLogsFunc, getEmptyRecordFunc, _ => "", startDateTime, endDateTime, interval))
		{
			memoryOutput.Logs.Add(log);
		}

		return memoryOutput;
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
			return new PartitionLog { Date = logs.First().Date, Bytes = bytes, UsedPercentage = usage };
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