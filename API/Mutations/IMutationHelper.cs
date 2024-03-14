using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Mutations;

public interface IMutationHelper
{
    Task<Payload<TOutput>> UpdateDbRecordAsync<TDbModel, TOutput>(Task<int> task, IQueryable<TDbModel> selectUpdatedObjectsQuery) where TDbModel : notnull;
    Task<Payload<TOutput>> AddModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> _getModelQuery);
    Task<Payload<TOutput>> AddOrReplaceModelAsync<TIdInput, TDbModel, TOutput>(TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> _getModelQuery);

    Task<Payload<TOutput>> DeleteModelAsync<TIdInput, TDbModel, TOutput>(TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> _getModelQuery);
    string GetGenericDatabaseErrorString() => "Database error";
    string DateToString(DateTime date);
}