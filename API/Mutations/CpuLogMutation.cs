using DataModel;
using HotChocolate.Language;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class CpuLogMutation(IDb db, IMutationHelper mutationHelper)
{
	private readonly Func<CpuLogIdInput, IQueryable<CpuLogDbRecord>> _getCpuLogQuery = cpuLog =>
        from c in db.CpuLogs
        where c.ServerId == cpuLog.ServerId && c.Date == cpuLog.Date
        select c;

    private readonly Func<CpuLogDbRecord, CpuLogIdInput> _getCpuLogId = cpuLog => new CpuLogIdInput(cpuLog.ServerId, cpuLog.Date);
    
    public async Task<Payload<CpuLog>> AddCpuLog(CpuLogInput log)
    {
        var model = InputToDbModel(log);
        return await mutationHelper.AddModelAsync<CpuLogIdInput, CpuLogDbRecord, CpuLog>(model, _getCpuLogId(model), _getCpuLogQuery);
    }

    public async Task<Payload<CpuLog>> AddOrUpdateCpuLog(CpuLogInput log)
    {
        var model = InputToDbModel(log);
        return await mutationHelper.AddOrReplaceModelAsync<CpuLogIdInput, CpuLogDbRecord, CpuLog>(model, _getCpuLogId(model), _getCpuLogQuery);
    }

    public async Task<Payload<CpuLog>> DeleteCpuLog(CpuLogIdInput logId)
    {
        return await mutationHelper.DeleteModelAsync<CpuLogIdInput, CpuLogDbRecord, CpuLog>(logId, _getCpuLogQuery);
    }

    private static CpuLogDbRecord InputToDbModel(CpuLogInput l)
    {
        var dbModel = new CpuLogDbRecord
        {
            ServerId = l.ServerId,
            Date = l.Date,
            Interval = l.Interval, 
            Usage = l.Usage,
            NumberOfTasks = l.NumberOfTasks
        };

        return dbModel;
    }
}