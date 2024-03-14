using DataModel;
using HotChocolate.Language;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class PartitionLogMutation(IDb db, IMutationHelper mutationHelper)
{
	private readonly Func<PartitionLogIdInput, IQueryable<PartitionLogDbRecord>> _getPartitionLogQuery = pId =>
		from p in db.PartitionLogs
		where p.Serverid == pId.ServerId && string.CompareOrdinal(p.Partitionuuid, pId.Uuid) == 0 && pId.Date == p.Date
		select p;

	private readonly Func<PartitionLogDbRecord, PartitionLogIdInput> _getPartitionLogId = p => new PartitionLogIdInput(p.Serverid, p.Partitionuuid, p.Date);
	
	public async Task<Payload<PartitionLog>> AddPartitionLog(PartitionLogInput partitionLog)
	{
		var model = InputToDbModel(partitionLog);
		return await mutationHelper.AddModelAsync<PartitionLogIdInput, PartitionLogDbRecord, PartitionLog>(model, _getPartitionLogId(model), _getPartitionLogQuery);
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