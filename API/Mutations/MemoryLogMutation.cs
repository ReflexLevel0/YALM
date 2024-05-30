using DataModel;
using HotChocolate.Language;
using YALM.API.Alerts;
using YALM.API.Db;
using YALM.Common;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class MemoryLogMutation(IDbProvider dbProvider, IMutationHelper mutationHelper, IAlertHelper alertHelper)
{
	private readonly Func<MemoryLogIdInput, IQueryable<MemoryLogDbRecord>> _getMemoryLogQuery = memoryLog =>
		from m in dbProvider.GetDb().MemoryLogs
		where m.ServerId == memoryLog.ServerId
		select m;

	private readonly Func<MemoryLogDbRecord, MemoryLogIdInput> _getMemoryLogId = memoryLog => new MemoryLogIdInput(memoryLog.ServerId);
	
	public async Task<Payload<MemoryLog>> AddMemoryLog(MemoryLogInput log)
	{
		var model = InputToDbModel(log);
		await AlertIfNeeded(log);
		return await mutationHelper.AddModelAsync<MemoryLogIdInput, MemoryLogDbRecord, MemoryLog>(model, _getMemoryLogId(model), _getMemoryLogQuery);
	}

	private async Task AlertIfNeeded(MemoryLogInput log)
	{
		if(log.UsedKb != null && log.TotalKb != null)
		{
			double usedPercentage = (double)log.UsedKb / (double)log.TotalKb;
			switch (usedPercentage)
			{
				case > 0.9:
					await alertHelper.RaiseAlert(log.ServerId, log.Date, AlertSeverity.Critical, "Memory usage above 90%");
					break;
				case > 0.75:
					await alertHelper.RaiseAlert(log.ServerId, log.Date, AlertSeverity.Warning, "Memory usage above 75%");
					break;
			}
		}
	}

	private static MemoryLogDbRecord InputToDbModel(MemoryLogInput l)
	{
		return new MemoryLogDbRecord
		{
			ServerId = l.ServerId,
			Date = l.Date,
			Interval = l.Interval,
			AvailableKb = l.AvailableKb,
			CachedKb = l.CachedKb,
			TotalKb = l.TotalKb,
			UsedKb = l.UsedKb,
			FreeKb = l.FreeKb,
			SwapTotalKb = l.SwapTotalKb,
			SwapUsedKb = l.SwapUsedKb,
			SwapFreeKb = l.SwapFreeKb
		};
	}
}