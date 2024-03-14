using DataModel;
using HotChocolate.Language;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class CpuMutation(IDb db, IMutationHelper mutationHelper)
{
    private readonly Func<CpuIdInput, IQueryable<CpuDbRecord>> _getCpuQuery = cpu =>
        from c in db.Cpus
        where c.ServerId == cpu.ServerId
        select c;

    private readonly Func<CpuDbRecord, CpuIdInput> _getCpuId = cpu => new CpuIdInput(cpu.ServerId);
    
    public async Task<Payload<CpuOutputBase>> AddCpu(CpuInput cpu)
    {
        var model = CpuInputToDbModel(cpu);
        return await mutationHelper.AddModelAsync<CpuIdInput, CpuDbRecord, CpuOutputBase>(model, _getCpuId(model), _getCpuQuery);
    }

    public async Task<Payload<CpuOutputBase>> AddOrUpdateCpu(CpuInput cpu)
    {
        var model = CpuInputToDbModel(cpu);
        return await mutationHelper.AddOrReplaceModelAsync<CpuIdInput, CpuDbRecord, CpuOutputBase>(model, _getCpuId(model), _getCpuQuery);
    }

    public async Task<Payload<CpuOutputBase>> DeleteCpu(CpuIdInput cpuId)
    {
        return await mutationHelper.DeleteModelAsync<CpuIdInput, CpuDbRecord, CpuOutputBase>(cpuId, _getCpuQuery);
    }

    // public async Task<Payload<CpuLog>> AddCpuLog(CpuLogInput cpuLog)
    // {
    //     var dbModel = new CpuLogDbRecord
    //     {
    //         Date = cpuLog.Date,
    //         Interval = cpuLog.Interval,
    //         Usage = cpuLog.Usage,
    //         ServerId = cpuLog.ServerId,
    //         NumberOfTasks = cpuLog.NumberOfTasks
    //     };
    //
    //     var query =
    //         from l in db.CpuLogs
    //         where l.ServerId == cpuLog.ServerId && l.Date == cpuLog.Date
    //         select l;
    //
    //     try
    //     {
    //         return await mutationHelper.UpdateDbRecordAsync<CpuLogDbRecord, CpuLog>(db.InsertAsync(dbModel), query);
    //     }
    //     catch
    //     {
    //         return new Payload<CpuLog> { Error = mutationHelper.GetGenericDatabaseErrorString() };
    //     }
    // }

    private static CpuDbRecord CpuInputToDbModel(CpuInput cpu)
    {
        var dbModel = new CpuDbRecord
        {
            ServerId = cpu.ServerId,
            Architecture = cpu.Architecture,
            Name = cpu.Name,
            Cores = cpu.Cores,
            Threads = cpu.Threads,
            FrequencyMhz = cpu.FrequencyMhz
        };

        return dbModel;
    }
}