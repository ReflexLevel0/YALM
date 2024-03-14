using DataModel;
using HotChocolate.Language;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class MemoryLogMutation(IDb db, IMutationHelper mutationHelper)
{
	private readonly Func<MemoryLogIdInput, IQueryable<MemoryLogDbRecord>> _getMemoryLogQuery = memoryLog =>
		from m in db.MemoryLogs
		where m.ServerId == memoryLog.ServerId
		select m;

	private readonly Func<MemoryLogDbRecord, MemoryLogIdInput> _getMemoryLogId = memoryLog => new MemoryLogIdInput(memoryLog.ServerId);
	
	public async Task<Payload<MemoryLog>> AddMemoryLog(MemoryLogInput memoryLog)
	{
		var model = InputToDbModel(memoryLog);
		return await mutationHelper.AddModelAsync<MemoryLogIdInput, MemoryLogDbRecord, MemoryLog>(model, _getMemoryLogId(model), _getMemoryLogQuery);
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