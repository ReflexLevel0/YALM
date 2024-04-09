using DataModel;
using HotChocolate.Language;
using YALM.API.Db.Models;
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
        var model = InputToDbModel(cpu);
        return await mutationHelper.AddModelAsync<CpuIdInput, CpuDbRecord, CpuOutputBase>(model, _getCpuId(model), _getCpuQuery);
    }

    public async Task<Payload<CpuOutputBase>> AddOrReplaceCpu(CpuInput cpu)
    {
        var model = InputToDbModel(cpu);
        return await mutationHelper.AddOrReplaceModelAsync<CpuIdInput, CpuDbRecord, CpuOutputBase>(model, _getCpuId(model), _getCpuQuery);
    }

    public async Task<Payload<CpuOutputBase>> DeleteCpu(CpuIdInput cpuId)
    {
        return await mutationHelper.DeleteModelAsync<CpuIdInput, CpuDbRecord, CpuOutputBase>(cpuId, _getCpuQuery);
    }

    private static CpuDbRecord InputToDbModel(CpuInput cpu)
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