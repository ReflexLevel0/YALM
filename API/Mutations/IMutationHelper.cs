using YALM.Common.Models.Graphql;

namespace YALM.API.Mutations;

public interface IMutationHelper
{
    Task<Payload<TOutput>> UpdateDbRecordAsync<TDbModel, TOutput>(Task<int> task, IQueryable<TDbModel> selectUpdatedObjectsQuery) where TDbModel : notnull;
    Task<Payload<TOutput>> AddModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;
    Task<Payload<TOutput>> AddOrReplaceModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;

    Task<Payload<TOutput>> DeleteModelAsync<TIdInput, TDbModel, TOutput>(TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;
    string GetGenericDatabaseErrorString() => "Database error";
    string DateToString(DateTime date);
}