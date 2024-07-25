using DataModel;
using HotChocolate.Language;
using LinqToDB;
using YALM.API.Db;
using YALM.API.Db.Models;
using YALM.API.GraphQL.Alerts;
using YALM.Common;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API.GraphQL.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class PartitionLogMutation(IMutationHelper mutationHelper, IAlertHelper alertHelper, IDbProvider dbProvider)
{
	private readonly Func<IDb, PartitionLogIdInput, IQueryable<PartitionLogDbRecord>> _getPartitionLogQuery = (db, pId) =>
		from p in db.PartitionLogs
		where p.Serverid == pId.ServerId && string.CompareOrdinal(p.Partitionuuid, pId.Uuid) == 0 && pId.Date == p.Date
		select p;

	private readonly Func<PartitionLogDbRecord, PartitionLogIdInput> _getPartitionLogId = p => new PartitionLogIdInput(p.Serverid, p.Partitionuuid, p.Date);
	
	public async Task<Payload<PartitionLog>> AddPartitionLog(PartitionLogInput log)
	{
		var model = InputToDbModel(log);
		await AlertIfNeeded(log);
		return await mutationHelper.AddModelAsync<PartitionLogIdInput, PartitionLogDbRecord, PartitionLog>(model, _getPartitionLogId(model), _getPartitionLogQuery);
	}
	
	public async Task<Payload<List<PartitionLog>>> AddPartitionLogs(List<PartitionLogInput> logs)
	{
		var partitionLogDbRecords = new List<PartitionLogDbRecord>();
		foreach (var p in logs)
		{
			await AlertIfNeeded(p);
		}
		logs.ForEach(p => partitionLogDbRecords.Add(InputToDbModel(p)));
		return await mutationHelper.AddOrReplaceModelsAsync<PartitionLogIdInput, PartitionLogDbRecord, PartitionLog>(partitionLogDbRecords, _getPartitionLogId, _getPartitionLogQuery);
	}

	private async Task AlertIfNeeded(PartitionLogInput log)
	{
		await using var db = dbProvider.GetDb();
		var partition = await (from p in db.Partitions
			where p.Uuid == log.PartitionUuid
			select p).FirstOrDefaultAsync();
		string partitionLabel = partition != null && partition.Label != null ? partition.Label : log.PartitionUuid;
		
		if (log.UsedPercentage != null)
		{
			switch (log.UsedPercentage)
			{
				case > (decimal)0.9:
					await alertHelper.RaiseAlert(log.ServerId, log.Date, AlertSeverity.Critical, $"Partition {partitionLabel} usage above 90%");
					break;
				case > (decimal)0.75:
					await alertHelper.RaiseAlert(log.ServerId, log.Date, AlertSeverity.Warning, $"Partition {partitionLabel} usage above 75%");
					break;
			}
		}
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