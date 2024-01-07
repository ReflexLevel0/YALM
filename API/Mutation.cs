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
	private readonly Func<CpuInput, IQueryable<CpuDbRecord>> _getCpuQuery = cpu =>
		from c in db.Cpus
		where c.ServerId == cpu.ServerId
		select c;

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

	public async Task<Payload<CpuOutputBase>> UpdateCpu(CpuKeyInput? oldCpuId, CpuInput newCpu)
	{
		//If no oldCpuId is specified, new processor will be created
		if (oldCpuId == null)
		{
			var dbModel = CpuInputToDbModel(newCpu);
			return await UpdateDbRecord<CpuDbRecord, CpuOutputBase>(db.InsertAsync(dbModel), _getCpuQuery(newCpu));
		}

		//If oldCpuId is specified, the old one is updated
		var task = db.Cpus.Where(c => c.ServerId == oldCpuId.ServerId)
			.Set(c => c.ServerId, newCpu.ServerId)
			.Set(c => c.Architecture, newCpu.Architecture)
			.Set(c => c.Name, newCpu.Name)
			.Set(c => c.Cores, newCpu.Cores)
			.Set(c => c.Threads, newCpu.Threads)
			.Set(c => c.FrequencyMhz, newCpu.FrequencyMhz)
			.UpdateAsync();

		try
		{
			return await UpdateDbRecord<CpuDbRecord, CpuOutputBase>(task, _getCpuQuery(newCpu));
		}
		catch
		{
			return new Payload<CpuOutputBase> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<CpuOutputBase>> DeleteCpu(CpuKeyInput cpuId)
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
			.Value(l => l.Date, memoryLog.Date)
			.Value(l => l.ServerId, memoryLog.ServerId)
			.Value(l => l.UsedPercentage, memoryLog.UsedPercentage)
			.Value(l => l.Interval, memoryLog.Interval)
			.Value(l => l.SwapUsedKb, memoryLog.SwapUsedKb)
			.Value(l => l.CachedKb, memoryLog.CachedKb)
			.Value(l => l.UsedKb, memoryLog.UsedKb)
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
		int? diskId = await
			(from p in db.Partitions
				join d in db.Disks on p.DiskId equals d.Id
				where partitionLog.ServerId == d.ServerId && string.CompareOrdinal(partitionLog.Uuid, p.Uuid) == 0
				select d.Id)
			.FirstOrDefaultAsync();

		if (diskId == null) throw new Exception("Partition not found!");

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
		var dbModel = new DiskDbRecord
		{
			Label = disk.Label,
			ServerId = disk.ServerId
		};

		var query =
			from d in db.Disks
			where d.ServerId == disk.ServerId && string.CompareOrdinal(d.Label, disk.Label) == 0
			select d;

		try
		{
			return await UpdateDbRecord<DiskDbRecord, DiskOutputBase>(db.InsertAsync(dbModel), query);
		}
		catch
		{
			return new Payload<DiskOutputBase> { Error = GenericDatabaseErrorString };
		}
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

	private static string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}