using System.Reflection;
using DataModel;
using LinqToDB;
using LinqToDB.Mapping;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API;

public class Mutation(IDb db)
{
	public async Task<Payload<CpuOutputBase>> AddCpu(CpuInput cpu)
	{
		var dbModel = new CpuDbRecord
		{
			ServerId = cpu.ServerId,
			Architecture = cpu.Architecture,
			Name = cpu.Name,
			Cores = cpu.Cores,
			Threads = cpu.Threads,
			FrequencyMhz = cpu.FrequencyMhz
		};
		
		var query =
			from c in db.Cpus
			where c.ServerId == cpu.ServerId
			select c;
		
		return await InsertRecordIntoDbAsync<CpuDbRecord, CpuOutputBase>(db.InsertAsync(dbModel), query);
	}
	
	public async Task<Payload<CpuLog>> AddCpuLog(CpuLogInput cpuLog)
	{
		var dbModel = new CpuLogDbRecord
		{
			Date = cpuLog.Date,
			Interval = cpuLog.Interval,
			Usage = cpuLog.Usage,
			ServerId = cpuLog.ServerId,
			NumberOfTasks = cpuLog.NumberOfTasks
		};

		var query =
			from l in db.CpuLogs
			where l.ServerId == cpuLog.ServerId && l.Date == cpuLog.Date
			select l;

		return await InsertRecordIntoDbAsync<CpuLogDbRecord, CpuLog>(db.InsertAsync(dbModel), query);
	}

	public async Task<Payload<MemoryLog>> AddMemoryLog(MemoryLogInput memoryLog)
	{
		var query =
			from l in db.MemoryLogs
			where l.ServerId == memoryLog.ServerId && l.Date == memoryLog.Date
			select l;

		var insertTask = db.MemoryLogs
			.Value(l => l.Date, memoryLog.Date)
			.Value(l => l.ServerId, memoryLog.ServerId)
			.Value(l => l.UsedPercentage, memoryLog.UsedPercentage)
			.Value(l => l.Interval, memoryLog.Interval)
			.Value(l => l.SwapUsedKb, memoryLog.SwapUsedKb)
			.Value(l => l.CachedKb, memoryLog.CachedKb)
			.Value(l => l.UsedKb, memoryLog.UsedKb)
			.InsertAsync();
		
		return await InsertRecordIntoDbAsync<MemoryLogDbRecord, MemoryLog>(insertTask, query);
	}
	
	public async Task<Payload<PartitionLog>> AddPartitionLog(PartitionLogInput partitionLog)
	{
		int? diskId = await
			(from p in db.Partitions 
				join d in db.Disks on p.DiskId equals d.Id 
				where partitionLog.ServerId == d.ServerId && string.CompareOrdinal(partitionLog.Uuid, p.Uuid) == 0 
				select d.Id)
			.FirstOrDefaultAsync();

		if(diskId == null) throw new Exception("Partition not found!");
		
		var query =
			from l in db.PartitionLogs
			where l.Uuid == partitionLog.Uuid && l.Date == partitionLog.Date
			select l;

		var insertTask = db.PartitionLogs
			.Value(l => l.Date, partitionLog.Date)
			.Value(l => l.Interval, partitionLog.Interval)
			.Value(l => l.Uuid, partitionLog.Uuid)
			.Value(l => l.DiskId, diskId)
			.Value(l => l.Usage, partitionLog.UsedPercentage)
			.Value(l => l.BytesTotal, partitionLog.Bytes)
			.InsertAsync();
		
		return await InsertRecordIntoDbAsync<PartitionLogDbRecord, PartitionLog>(insertTask, query);
	}

	public async Task<Payload<PartitionOutputBase>> AddPartition(PartitionInput partition)
	{
		int? diskId = await (
			from d in db.Disks
			where string.CompareOrdinal(d.Label, partition.DiskLabel) == 0
			select d.Id).FirstOrDefaultAsync();
		if (diskId == null) throw new Exception("Disk ID not found");
		
		var dbModel = new PartitionDbRecord
		{
			DiskId = (int)diskId,
			Uuid = partition.Uuid,
			Label = partition.PartitionLabel,
			FilesystemName = partition.FilesystemName,
			FilesystemVersion = partition.FilesystemVersion,
			MountPath = partition.Mountpath
		};
	
		var query =
			from p in db.Partitions
			where p.DiskId == diskId && string.CompareOrdinal(p.Uuid, partition.Uuid) == 0
			select p;
	
		var insertTask = db.Partitions
			.Value(p => p.FilesystemName, dbModel.FilesystemName)
			.Value(p => p.FilesystemVersion, dbModel.FilesystemVersion)
			.Value(p => p.Uuid, dbModel.Uuid)
			.Value(p => p.DiskId, diskId)
			.Value(p => p.Label, dbModel.Label)
			.Value(p => p.MountPath, dbModel.MountPath)
			.InsertAsync();
		
		return await InsertRecordIntoDbAsync<PartitionDbRecord, PartitionOutputBase>(insertTask, query);
	}

	public async Task<Payload<DiskOutputBase>> AddDisk(DiskInput disk)
	{
		var dbModel = new DiskDbRecord
		{
			Label = disk.Label,
			ServerId = disk.ServerId
		};

		var query =
			from d in db.Disks
			where d.ServerId == disk.ServerId && string.CompareOrdinal(d.Label, disk.Label) == 0
			select d;

		return await InsertRecordIntoDbAsync<DiskDbRecord, DiskOutputBase>(db.InsertAsync(dbModel), query);
	}

	private async Task<Payload<TOutput>> InsertRecordIntoDbAsync<TDbModel, TOutput>(Task<int> insertTask, IQueryable<TDbModel> selectQuery) where TDbModel : notnull
	{
		var tableAttr = typeof(TDbModel).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
		var errorPayload = new Payload<TOutput> { Error = GenerateInsertError(tableAttr?.Name) };

		//Inserting data into the database
		int affectedRecords = await insertTask;
		if (affectedRecords == 0) return errorPayload;

		//Getting the inserted record and returning it (or returning error if data couldn't be fetched)
		var log = (await selectQuery.ToListAsync()).FirstOrDefault();
		return log == null
			? errorPayload
			: new Payload<TOutput>
			{
				Data = (TOutput)Convert.ChangeType(log, typeof(TOutput))
			};
	}

	private static string GenerateInsertError(string? type) => $"Failed to insert {type ?? "object"}";

	private static string? GetTableNameFromType(ICustomAttributeProvider type)
	{
		var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
		return tableAttr?.Name;
	}

	private static string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}