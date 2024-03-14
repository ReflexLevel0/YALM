using DataModel;
using LinqToDB;
using LinqToDB.Mapping;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

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