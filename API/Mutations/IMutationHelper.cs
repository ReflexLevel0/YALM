using YALM.Common.Models.Graphql;

namespace YALM.API.Mutations;

public interface IMutationHelper
{
    /// <summary>
    /// Adds a model to the database
    /// </summary>
    /// <param name="model">Model to be added</param>
    /// <param name="modelId">Primary key of the model</param>
    /// <param name="getModelQuery">Function to get the model based on ID</param>
    /// <typeparam name="TIdInput">Type of class containting ID information about the model</typeparam>
    /// <typeparam name="TDbModel">Type of the model in the database</typeparam>
    /// <typeparam name="TOutput">Type of data to be returned</typeparam>
    /// <returns></returns>
    Task<Payload<TOutput>> AddModelAsync<TIdInput, TDbModel, TOutput>(
        TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;

    Task<Payload<List<TOutput>>> AddModelsAsync<TIdInput, TDbModel, TOutput>(
        List<TDbModel> models, Func<TDbModel, TIdInput> getModelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;
    
    Task<Payload<TOutput>> AddOrReplaceModelAsync<TIdInput, TDbModel, TOutput>(
        TDbModel model, TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;
    
    Task<Payload<List<TOutput>>> AddOrReplaceModelsAsync<TIdInput, TDbModel, TOutput>(
        IEnumerable<TDbModel> models, Func<TDbModel, TIdInput> getModelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;

    Task<Payload<TOutput>> DeleteModelAsync<TIdInput, TDbModel, TOutput>(
        TIdInput modelId, Func<TIdInput, IQueryable<TDbModel>> getModelQuery) where TDbModel : notnull;
    
    string GetGenericDatabaseErrorString() => "Database error";
    
    string DateToString(DateTime date);
}