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
	private readonly Func<CpuIdInput, IQueryable<CpuDbRecord>> _getCpuQuery = cpu =>
		from c in db.Cpus
		where c.ServerId == cpu.ServerId
		select c;
	
	private readonly Func<DiskIdInput, IQueryable<DiskDbRecord>> _getDiskQuery = disk =>
		from d in db.Disks
		where d.ServerId == disk.ServerId && string.CompareOrdinal(d.Uuid, disk.Uuid) == 0
		select d;

	private const string GenericDatabaseErrorString = "Database error";

	public async Task<Payload<CpuOutputBase>> AddCpu(CpuInput cpu)
	{
		var dbModel = CpuInputToDbModel(cpu);
		try
		{
			return await UpdateDbRecord<CpuDbRecord, CpuOutputBase>(db.InsertAsync(dbModel), _getCpuQuery(cpu));
		}
		catch
		{
			return new Payload<CpuOutputBase> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<CpuOutputBase>> AddOrUpdateCpu(CpuInput cpu)
	{
		var cpuDbModel = CpuInputToDbModel(cpu);

		try
		{
			await db.InsertOrReplaceAsync(cpuDbModel);
			var cpuDbRecord = await _getCpuQuery(cpu).FirstAsync();
			return new Payload<CpuOutputBase> { Data = (CpuOutputBase)Convert.ChangeType(cpuDbRecord, typeof(CpuOutputBase)) };
		}
		catch
		{
			return new Payload<CpuOutputBase> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<CpuOutputBase>> DeleteCpu(CpuIdInput cpuId)
	{
		var cpu = await
			(from c in db.Cpus
				where c.ServerId == cpuId.ServerId
				select c).FirstOrDefaultAsync();
		if (cpu == null) return new Payload<CpuOutputBase> { Error = "Cpu not found" };
		var selectQuery = from c in db.Cpus where c.ServerId == cpuId.ServerId select c;

		try
		{
			return await UpdateDbRecord<CpuDbRecord, CpuOutputBase>(db.DeleteAsync(cpu), selectQuery);
		}
		catch
		{
			return new Payload<CpuOutputBase> { Error = GenericDatabaseErrorString };
		}
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

		try
		{
			return await UpdateDbRecord<CpuLogDbRecord, CpuLog>(db.InsertAsync(dbModel), query);
		}
		catch
		{
			return new Payload<CpuLog> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<MemoryLog>> AddMemoryLog(MemoryLogInput memoryLog)
	{
		var query =
			from l in db.MemoryLogs
			where l.ServerId == memoryLog.ServerId && l.Date == memoryLog.Date
			select l;

		var insertTask = db.MemoryLogs
			.Value(l => l.ServerId, memoryLog.ServerId)
			.Value(l => l.Date, memoryLog.Date)
			.Value(l => l.Interval, memoryLog.Interval)
			.Value(l => l.TotalKb, memoryLog.TotalKb)
			.Value(l => l.FreeKb, memoryLog.FreeKb)
			.Value(l => l.UsedKb, memoryLog.UsedKb)
			.Value(l => l.SwapTotalKb, memoryLog.SwapTotalKb)
			.Value(l => l.SwapFreeKb, memoryLog.SwapFreeKb)
			.Value(l => l.SwapUsedKb, memoryLog.SwapUsedKb)
			.Value(l => l.CachedKb, memoryLog.CachedKb)
			.Value(l => l.AvailableKb, memoryLog.AvailableKb)
			.InsertAsync();

		try
		{
			return await UpdateDbRecord<MemoryLogDbRecord, MemoryLog>(insertTask, query);
		}
		catch
		{
			return new Payload<MemoryLog> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<PartitionLog>> AddPartitionLog(PartitionLogInput partitionLog)
	{
		string? diskUuid = await
			(from p in db.Partitions
				join d in db.Disks on p.Diskuuid equals d.Uuid
				where partitionLog.ServerId == d.ServerId && string.CompareOrdinal(partitionLog.PartitionUuid, p.Uuid) == 0
				select d.Uuid)
			.FirstOrDefaultAsync();
	
		if (string.IsNullOrEmpty(diskUuid)) throw new Exception("Partition not found!");
	
		var query =
			from l in db.PartitionLogs
			where l.Partitionuuid == partitionLog.PartitionUuid && l.Date == partitionLog.Date
			select l;
	
		var insertTask = db.PartitionLogs
			.Value(l => l.Serverid, partitionLog.ServerId)
			.Value(l => l.Date, partitionLog.Date)
			.Value(l => l.Interval, partitionLog.Interval)
			.Value(l => l.Partitionuuid, partitionLog.PartitionUuid)
			.Value(l => l.Usage, partitionLog.UsedPercentage)
			.Value(l => l.BytesTotal, partitionLog.Bytes)
			.InsertAsync();
	
		try
		{
			return await UpdateDbRecord<PartitionLogDbRecord, PartitionLog>(insertTask, query);
		}
		catch
		{
			return new Payload<PartitionLog> { Error = GenericDatabaseErrorString };
		}
	}
	
	public async Task<Payload<PartitionOutputBase>> AddPartition(PartitionInput partition)
	{
		string? diskUuid = await (
			from d in db.Disks
			where d.ServerId == partition.ServerId && string.CompareOrdinal(d.Uuid, partition.DiskUuid) == 0
			select d.Uuid).FirstOrDefaultAsync();
		if (diskUuid == null) throw new Exception("Disk ID not found");
	
		var dbModel = new PartitionDbRecord
		{
			Serverid = partition.ServerId,
			Diskuuid = diskUuid,
			Uuid = partition.Uuid,
			Label = partition.PartitionLabel,
			FilesystemName = partition.FilesystemName,
			FilesystemVersion = partition.FilesystemVersion,
			MountPath = partition.Mountpath
		};
	
		var query =
			from p in db.Partitions
			where string.CompareOrdinal(p.Diskuuid, diskUuid) == 0 && string.CompareOrdinal(p.Uuid, partition.Uuid) == 0
			select p;
	
		var insertTask = db.Partitions
			.Value(p => p.Serverid, dbModel.Serverid)
			.Value(p => p.FilesystemName, dbModel.FilesystemName)
			.Value(p => p.FilesystemVersion, dbModel.FilesystemVersion)
			.Value(p => p.Uuid, dbModel.Uuid)
			.Value(p => p.Diskuuid, diskUuid)
			.Value(p => p.Label, dbModel.Label)
			.Value(p => p.MountPath, dbModel.MountPath)
			.InsertAsync();
	
		try
		{
			return await UpdateDbRecord<PartitionDbRecord, PartitionOutputBase>(insertTask, query);
		}
		catch
		{
			return new Payload<PartitionOutputBase> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<DiskOutputBase>> AddDisk(DiskInput disk)
	{
		var dbModel = DiskInputToDbModel(disk);

		try
		{
			return await UpdateDbRecord<DiskDbRecord, DiskOutputBase>(db.InsertAsync(dbModel), _getDiskQuery(new DiskIdInput(disk.ServerId, disk.Uuid)));
		}
		catch
		{
			return new Payload<DiskOutputBase> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<DiskOutputBase>> AddOrUpdateDisk(DiskInput disk)
	{
		var diskDbModel = DiskInputToDbModel(disk);
		
		try
		{
			await db.InsertOrReplaceAsync(diskDbModel);
			var diskDbRecord = await _getDiskQuery(new DiskIdInput(disk.ServerId, disk.Uuid)).FirstAsync();
			return new Payload<DiskOutputBase> { Data = (DiskOutputBase)Convert.ChangeType(diskDbRecord, typeof(DiskOutputBase)) };
		}
		catch
		{
			return new Payload<DiskOutputBase> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<List<DiskOutputBase>>> AddOrUpdateDisks(List<DiskInput> disks)
	{
		var payloadList = new List<DiskOutputBase>();
		string error = "";
		
		foreach (var disk in disks)
		{
			var payload = await AddOrUpdateDisk(disk);
			if(payload.Data != null) payloadList.Add(payload.Data);

			if (payload.Error == null) continue;
			error = payload.Error;
			payloadList = null;
			break;
		}

		return new Payload<List<DiskOutputBase>>{Data = payloadList, Error = error};
	}

	public async Task<Payload<ProgramLog>> AddProgramLog(ProgramLogInput programLog)
	{
		var programModel = new ProgramLogDbRecord
		{
			Serverid = programLog.ServerId,
			Date = programLog.Date,
			Interval = programLog.Interval,
			Name = programLog.Name,
			CpuutilizationPercentage = programLog.CpuUsage,
			MemoryUtilizationPercentage = programLog.MemoryUsage 
		};

		var query = 
			from l in db.ProgramLogs 
			where l.Serverid == programLog.ServerId && string.CompareOrdinal(l.Name, programLog.Name) == 0 && l.Date == programLog.Date
			select l;

		try
		{
			return await UpdateDbRecord<ProgramLogDbRecord, ProgramLog>(db.InsertAsync(programModel), query);
		}
		catch(Exception ex)
		{
			return new Payload<ProgramLog> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<List<ProgramLog>>> AddProgramLogs(List<ProgramLogInput> programLogs)
	{
		var logs = new List<ProgramLog>();
		foreach (var log in programLogs)
		{
			var l = await AddProgramLog(log);
			if (string.IsNullOrEmpty(l.Error) == false || l.Data == null)
			{
				return new Payload<List<ProgramLog>> { Error = GenericDatabaseErrorString };
			}

			logs.Add(l.Data);
		}

		return new Payload<List<ProgramLog>> { Data = logs };
	}

	private async Task<Payload<TOutput>> UpdateDbRecord<TDbModel, TOutput>(Task<int> task, IQueryable<TDbModel> selectUpdatedObjectsQuery) where TDbModel : notnull
	{
		var tableAttr = typeof(TDbModel).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
		var errorPayload = new Payload<TOutput> { Error = GenerateInsertError(tableAttr?.Name) };

		//Inserting data into the database
		int affectedRecords = await task;
		if (affectedRecords == 0) return errorPayload;

		//Getting the inserted record and returning it (or returning error if data couldn't be fetched)
		var log = await selectUpdatedObjectsQuery.FirstOrDefaultAsync();
		var payload = new Payload<TOutput>();
		if (log != null)
		{
			payload.Data = (TOutput)Convert.ChangeType(log, typeof(TOutput));
		}

		return payload;
	}

	private static string GenerateInsertError(string? type) => $"Failed to insert {type ?? "object"}";

	private static string? GetTableNameFromType(ICustomAttributeProvider type)
	{
		var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
		return tableAttr?.Name;
	}

	private static CpuDbRecord CpuInputToDbModel(CpuInput cpu)
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

		return dbModel;
	}

	private static DiskDbRecord DiskInputToDbModel(DiskInput disk)
	{
		var dbModel = new DiskDbRecord
		{
			Uuid = disk.Uuid,
			ServerId = disk.ServerId,
			BytesTotal = disk.BytesTotal,
			Model = disk.Model,
			Path = disk.Path,
			Serial = disk.Serial,
			Type = disk.Type,
			Vendor = disk.Vendor
		};

		return dbModel;
	}

	private static string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}