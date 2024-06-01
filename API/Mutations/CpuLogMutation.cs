using DataModel;
using HotChocolate.Language;
using YALM.API.Alerts;
using YALM.API.Db.Models;
using YALM.Common;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class CpuLogMutation(IMutationHelper mutationHelper, IAlertHelper alertHelper)
{
    private readonly Func<IDb, CpuLogIdInput, IQueryable<CpuLogDbRecord>> _getCpuLogQuery = (db, cpuLog) => 
        from c in db.CpuLogs
        where c.ServerId == cpuLog.ServerId && c.Date == cpuLog.Date
        select c;

    private readonly Func<CpuLogDbRecord, CpuLogIdInput> _getCpuLogId = cpuLog => new CpuLogIdInput(cpuLog.ServerId, cpuLog.Date);
    
    public async Task<Payload<CpuLog>> AddCpuLog(CpuLogInput log)
    {
        var model = InputToDbModel(log);
        await AlertIfNeeded(log);
        return await mutationHelper.AddModelAsync<CpuLogIdInput, CpuLogDbRecord, CpuLog>(model, _getCpuLogId(model), _getCpuLogQuery);
    }

    public async Task<Payload<CpuLog>> AddOrUpdateCpuLog(CpuLogInput log)
    {
        var model = InputToDbModel(log);
        await AlertIfNeeded(log);
        return await mutationHelper.AddOrReplaceModelAsync<CpuLogIdInput, CpuLogDbRecord, CpuLog>(model, _getCpuLogId(model), _getCpuLogQuery);
    }

    public async Task<Payload<CpuLog>> DeleteCpuLog(CpuLogIdInput logId)
    {
        return await mutationHelper.DeleteModelAsync<CpuLogIdInput, CpuLogDbRecord, CpuLog>(logId, _getCpuLogQuery);
    }

    private async Task AlertIfNeeded(CpuLogInput log)
    {
        switch (log.Usage)
        {
            case > 0.9:
                await alertHelper.RaiseAlert(log.ServerId, log.Date, AlertSeverity.Critical, "Cpu usage above 90%");
                break;
            case > 0.75:
                await alertHelper.RaiseAlert(log.ServerId, log.Date, AlertSeverity.Warning, "Cpu usage above 75%");
                break;
        }
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