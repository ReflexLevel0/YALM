using DataModel;
using HotChocolate.Language;
using YALM.API.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class PartitionLogMutation(IDbProvider dbProvider, IMutationHelper mutationHelper)
{
	private readonly Func<PartitionLogIdInput, IQueryable<PartitionLogDbRecord>> _getPartitionLogQuery = pId =>
		from p in dbProvider.GetDb().PartitionLogs
		where p.Serverid == pId.ServerId && string.CompareOrdinal(p.Partitionuuid, pId.Uuid) == 0 && pId.Date == p.Date
		select p;

	private readonly Func<PartitionLogDbRecord, PartitionLogIdInput> _getPartitionLogId = p => new PartitionLogIdInput(p.Serverid, p.Partitionuuid, p.Date);
	
	public async Task<Payload<PartitionLog>> AddPartitionLog(PartitionLogInput log)
	{
		var model = InputToDbModel(log);
		return await mutationHelper.AddModelAsync<PartitionLogIdInput, PartitionLogDbRecord, PartitionLog>(model, _getPartitionLogId(model), _getPartitionLogQuery);
	}
	
	public async Task<Payload<List<PartitionLog>>> AddPartitionLogs(List<PartitionLogInput> logs)
	{
		var partitionLogDbRecords = new List<PartitionLogDbRecord>();
		logs.ForEach(p => partitionLogDbRecords.Add(InputToDbModel(p)));
		return await mutationHelper.AddOrReplaceModelsAsync<PartitionLogIdInput, PartitionLogDbRecord, PartitionLog>(partitionLogDbRecords, _getPartitionLogId, _getPartitionLogQuery);
	}

	private static PartitionLogDbRecord InputToDbModel(PartitionLogInput l)
	{
		return new PartitionLogDbRecord
		{
			Serverid = l.ServerId,
			Partitionuuid = l.PartitionUuid,
			Date = l.Date,
			Interval = l.Interval,
			Usage = l.UsedPercentage,
			BytesTotal = l.Bytes
		};
	}
}