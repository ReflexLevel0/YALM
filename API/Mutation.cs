using System.Reflection;
using DataModel;
using LinqToDB;
using LinqToDB.Mapping;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API;

public class Mutation(IDb db)
{ 
	public async Task<Payload<CpuLog>> AddCpuLog(CpuLogInput cpuLog)
	{
		var errorPayload = new Payload<CpuLog> { Error = GenerateInsertError(typeof(CpuLogDbRecord)) };
		
		int affectedRecords = await db.InsertAsync(new CpuLogDbRecord
		{
			Date = cpuLog.Date,
			Interval = cpuLog.Interval,
			Usage = cpuLog.Usage,
			ServerId = cpuLog.ServerId,
			NumberOfTasks = cpuLog.NumberOfTasks
		});
		
		if (affectedRecords == 0) return errorPayload;
		
		var query = 
			from l in db.CpuLogs 
			where l.ServerId == cpuLog.ServerId && l.Date == cpuLog.Date 
			select l;
		
		var log = (await query.ToListAsync()).FirstOrDefault();
		return log == null ? errorPayload : new Payload<CpuLog> {Data = (CpuLog)log };
	}
	
	// public async Task<Payload<MemoryLog>> AddMemoryLog(MemoryLogInput memoryLog)
	// {
	// 	string date = DateToString(memoryLog.Date);
	// 	var payload = await ExecuteInsertQuery<MemoryLogInput, MemoryLogInput, MemoryLog>(
	// 		reader =>
	// 		{
	// 			var log = db.ParseMemoryLogRecord(reader);
	// 			return new MemoryLogInput(log.ServerId, log.Interval, log.Date, log.MbUsed, log.MbTotal);
	// 		},
	// 		objects =>
	// 		{
	// 			if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
	// 			var obj = objects.First();
	// 			return new MemoryLog(obj.Date, obj.MbUsed, obj.MbTotal);
	// 		},
	// 		$"INSERT INTO Memory(serverId, date, interval, mbUsed, mbTotal)" +
	// 		$"VALUES({memoryLog.ServerId}, '{date}', {memoryLog.Interval}, {memoryLog.MbUsed}, {memoryLog.MbTotal})",
	// 		$"SELECT serverid, date, interval, mbUsed, mbTotal " +
	// 		$"FROM Memory WHERE serverId = {memoryLog.ServerId} AND date = '{date}'");
	// 	return payload;
	// }
	//
	// public async Task<Payload<PartitionLog>> AddPartitionLog(PartitionLogInput partitionLog)
	// {
	// 	string date = DateToString(partitionLog.Date);
	// 	var payload = await ExecuteInsertQuery<PartitionLogInput, PartitionLogInput, PartitionLog>(
	// 		reader =>
	// 		{
	// 			var log = db.ParsePartitionLogRecord(reader);
	// 			return new PartitionLogInput(log.DiskId, log.Date, log.Interval, log.UUID, log.Bytes, log.UsedPercentage);
	// 		},
	// 		objects =>
	// 		{
	// 			if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
	// 			var obj = objects.First();
	// 			return new PartitionLog(obj.Date, obj.Bytes, obj.UsedPercentage);
	// 		},
	// 		"INSERT INTO partitionlog(diskid, uuid, date, interval, bytestotal, usage) " + 
	// 		$"VALUES({partitionLog.DiskId}, '{partitionLog.Uuid}', '{date}', {partitionLog.Interval}, {partitionLog.Bytes}, {partitionLog.UsedPercentage})",
	// 		"SELECT diskid, uuid, date, interval, bytestotal, usage " + 
	// 		$"FROM partitionlog WHERE diskid = {partitionLog.DiskId} AND uuid = '{partitionLog.Uuid}' AND date = '{date}'");
	// 	return payload;
	// }
	//
	// public async Task<Payload<PartitionOutputBase>> AddPartition(PartitionInput partition)
	// {
	// 	//Getting disk id for the specified disk label
	// 	int? diskId = null;
	// 	await foreach (var reader in db.ExecuteReaderAsync($"SELECT id FROM disk WHERE label = '{partition.DiskLabel}'"))
	// 	{
	// 		diskId = reader.GetInt32(0);
	// 		break;
	// 	}
	// 	
	// 	if (diskId == null) throw new Exception("Error in parsing disk data!");
	// 	
	// 	//Adding partition to the database
	// 	var payload = await ExecuteInsertQuery<PartitionInput, PartitionInput, PartitionOutputBase>(
	// 		reader =>
	// 		{
	// 			var log = db.ParsePartitionRecord(reader);
	// 			return new PartitionInput(partition.DiskLabel, log.Uuid, log.FilesystemName, log.FilesystemVersion, log.Label, log.MountPath);
	// 		},
	// 		objects =>
	// 		{
	// 			if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
	// 			var obj = objects.First();
	// 			return new PartitionOutputBase(obj.Uuid, obj.FilesystemName, obj.FilesystemVersion, obj.PartitionLabel, obj.Mountpath);
	// 		},
	// 		"INSERT INTO partition(diskid, uuid, filesystemname, filesystemversion, label, mountpath) " + 
	// 		$"VALUES({diskId}, '{partition.Uuid}', '{partition.FilesystemName}', '{partition.FilesystemVersion}', '{partition.PartitionLabel}', '{partition.Mountpath}')",
	// 		"SELECT uuid, filesystemname, filesystemversion, label, mountpath " + 
	// 		$"FROM partition"
	// 	);
	// 	return payload;
	// }
	//
	// public async Task<Payload<DiskOutputBase>> AddDisk(DiskInput disk)
	// {
	// 	var payload = await ExecuteInsertQuery<DiskInput, DiskOutputBase, DiskOutputBase>(
	// 		reader =>
	// 		{
	// 			var log = db.ParseDiskRecord(reader);
	// 			return new DiskOutputBase(log.ServerId, log.Label);
	// 		},
	// 		objects =>
	// 		{
	// 			if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
	// 			return objects.First();
	// 		},
	// 		"INSERT INTO disk(serverid, label) " + 
	// 		$"VALUES({disk.ServerId}, '{disk.Label}')",
	// 		"SELECT serverId, label " + 
	// 		$"FROM disk WHERE serverid = {disk.ServerId} AND label = '{disk.Label}'");
	// 	return payload;
	// }

	private static string GenerateInsertError(MemberInfo type) => $"Failed to insert {type.Name}";
	
	private static string? GetTableNameFromType(ICustomAttributeProvider type)
	{
		var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
		return tableAttr?.Name;
	}
	
	private static string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}