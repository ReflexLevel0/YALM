using DataModel;
using LinqToDB;
using LinqToDB.Mapping;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Mutations;

public class MutationHelper(IDb db) : IMutationHelper
{
	public async Task<Payload<TOutput>> AddModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull 
	{
		try
		{
			return await UpdateDbRecordAsync<TDbModel, TOutput>(db.InsertAsync(model), getModelQuery(modelId));
		}
		catch
		{
			return new Payload<TOutput> { Error = GetGenericDatabaseErrorString() };
		}
	}

	public async Task<Payload<TOutput>> AddOrReplaceModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull
	{
		try
		{
			await db.InsertOrReplaceAsync(model);
			var dbRecord = await getModelQuery(modelId).FirstAsync();
			return new Payload<TOutput> { Data = (TOutput)Convert.ChangeType(dbRecord, typeof(TOutput)) };
		}
		catch
		{
			return new Payload<TOutput> { Error = GetGenericDatabaseErrorString() };
		}
	}

	public async Task<Payload<List<TOutput>>> AddOrReplaceModelsAsync<TIdInput, TDbModel, TOutput>(IEnumerable<TDbModel> models, Func<TDbModel, TIdInput> getModelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull
	{
		var payloadList = new List<TOutput>();
		string error = "";
		
		foreach (var model in models)
		{
			var payload = await AddOrReplaceModelAsync<TIdInput, TDbModel, TOutput>(model, getModelId(model), getModelQuery);
			if(payload.Data != null) payloadList.Add(payload.Data);
	
			if (payload.Error == null) continue;
			error = payload.Error;
			payloadList = null;
			break;
		}
	
		return new Payload<List<TOutput>>{Data = payloadList, Error = error};
	}
	
	public async Task<Payload<TOutput>> DeleteModelAsync<TIdInput, TDbModel, TOutput>(TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull
	{
		try
		{
			var model = await getModelQuery(modelId).FirstAsync();
			await db.DeleteAsync(model);
			return new Payload<TOutput> { Data = (TOutput)Convert.ChangeType(model, typeof(TOutput)) };
		}
		catch
		{
			return new Payload<TOutput> { Error = GetGenericDatabaseErrorString() };
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
			return await UpdateDbRecordAsync<PartitionLogDbRecord, PartitionLog>(insertTask, query);
		}
		catch
		{
			return new Payload<PartitionLog> { Error = GetGenericDatabaseErrorString() };
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
			return await UpdateDbRecordAsync<PartitionDbRecord, PartitionOutputBase>(insertTask, query);
		}
		catch
		{
			return new Payload<PartitionOutputBase> { Error = GetGenericDatabaseErrorString() };
		}
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
			return await UpdateDbRecordAsync<ProgramLogDbRecord, ProgramLog>(db.InsertAsync(programModel), query);
		}
		catch
		{
			return new Payload<ProgramLog> { Error = GetGenericDatabaseErrorString() };
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
				return new Payload<List<ProgramLog>> { Error = GetGenericDatabaseErrorString() };
			}

			logs.Add(l.Data);
		}

		return new Payload<List<ProgramLog>> { Data = logs };
	}

	public async Task<Payload<TOutput>> UpdateDbRecordAsync<TDbModel, TOutput>(Task<int> task, IQueryable<TDbModel> selectUpdatedObjectsQuery) where TDbModel : notnull
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

	public string GetGenericDatabaseErrorString() => "Database error";
	
	public string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}