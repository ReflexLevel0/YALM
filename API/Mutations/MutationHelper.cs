using LinqToDB;
using LinqToDB.Mapping;
using YALM.API.Db;
using YALM.API.Db.Models;
using YALM.Common.Models.Graphql;

namespace YALM.API.Mutations;

public class MutationHelper(IDbProvider dbProvider) : IMutationHelper
{
	private const string GenericDatabaseErrorString = "Database error";
	
	public async Task<Payload<TOutput>> AddModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<IDb, TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull 
	{
		try
		{
			await using var db = dbProvider.GetDb();
			return await UpdateDbRecordAsync<TDbModel, TOutput>(db.InsertAsync(model), getModelQuery(db, modelId));
		}
		catch
		{
			return new Payload<TOutput> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<List<TOutput>>> AddModelsAsync<TIdInput, TDbModel, TOutput>(List<TDbModel> models, Func<TDbModel, TIdInput> getModelId, Func<IDb, TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull
	{
		return await BulkAction<TDbModel, TOutput>(models, model => AddModelAsync<TIdInput, TDbModel, TOutput>(model, getModelId(model), getModelQuery));
	}

	public async Task<Payload<TOutput>> AddOrReplaceModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<IDb, TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull
	{
		try
		{
			await using var db = dbProvider.GetDb();
			await db.InsertOrReplaceAsync(model);
			var dbRecord = await getModelQuery(db, modelId).FirstAsync();
			return new Payload<TOutput> { Data = (TOutput)Convert.ChangeType(dbRecord, typeof(TOutput)) };
		}
		catch(Exception ex)
		{
			Console.WriteLine(ex.Message);
			return new Payload<TOutput> { Error = GenericDatabaseErrorString };
		}
	}

	public async Task<Payload<List<TOutput>>> AddOrReplaceModelsAsync<TIdInput, TDbModel, TOutput>(IEnumerable<TDbModel> models, Func<TDbModel, TIdInput> getModelId, Func<IDb, TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull
	{
		await using var db = dbProvider.GetDb();
		return await BulkAction<TDbModel, TOutput>(models, model => AddOrReplaceModelAsync<TIdInput, TDbModel, TOutput>(model, getModelId(model), getModelQuery));
	}
	
	public async Task<Payload<TOutput>> DeleteModelAsync<TIdInput, TDbModel, TOutput>(TIdInput modelId, Func<IDb, TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull
	{
		try
		{
			await using var db = dbProvider.GetDb();
			var model = await getModelQuery(db, modelId).FirstAsync();
			await db.DeleteAsync(model);
			return new Payload<TOutput> { Data = (TOutput)Convert.ChangeType(model, typeof(TOutput)) };
		}
		catch
		{
			return new Payload<TOutput> { Error = GenericDatabaseErrorString };
		}
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

	private async Task<Payload<List<TOutput>>> BulkAction<TDbModel, TOutput>(IEnumerable<TDbModel> models, Func<TDbModel,Task<Payload<TOutput>>> execute) where TDbModel : notnull
	{
		var payloadList = new List<TOutput>();
		string error = "";
		
		foreach (var model in models)
		{
			var payload = await execute(model).WaitAsync(new CancellationToken());
			if(payload.Data != null) payloadList.Add(payload.Data);
	
			if (payload.Error == null) continue;
			error = payload.Error;
			payloadList = null;
			break;
		}
	
		return new Payload<List<TOutput>>{Data = payloadList, Error = error.Length == 0 ? null : error};
	}
	
	private static string GenerateInsertError(string? type) => $"Failed to insert {type ?? "object"}";
}