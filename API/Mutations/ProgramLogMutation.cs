using DataModel;
using HotChocolate.Language;
using YALM.API.Models.Db;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class ProgramLogMutation(IDb db, IMutationHelper mutationHelper)
{
	private readonly Func<ProgramLogIdInput, IQueryable<ProgramLogDbRecord>> _getProgramQuery = pid =>
		from p in db.ProgramLogs
		where pid.ServerId == p.Serverid && pid.Date == p.Date && string.CompareOrdinal(pid.Name, p.Name) == 0
		select p;

	private readonly Func<ProgramLogDbRecord, ProgramLogIdInput> _getProgramLogId = p => new ProgramLogIdInput(p.Serverid, p.Name, p.Date);

	public async Task<Payload<ProgramLog>> AddProgramLog(ProgramLogInput log)
	{
		var model = InputToDbModel(log);
		return await mutationHelper.AddModelAsync<ProgramLogIdInput, ProgramLogDbRecord, ProgramLog>(model, _getProgramLogId(model), _getProgramQuery);
	}

	public async Task<Payload<List<ProgramLog>>> AddProgramLogs(List<ProgramLogInput> logs)
	{
		var programInputs = new List<ProgramLogDbRecord>();
		logs.ForEach(l => programInputs.Add(InputToDbModel(l)));
		return await mutationHelper.AddModelsAsync<ProgramLogIdInput, ProgramLogDbRecord, ProgramLog>(programInputs, _getProgramLogId, _getProgramQuery);
	}

	private static ProgramLogDbRecord InputToDbModel(ProgramLogInput l)
	{
		return new ProgramLogDbRecord
		{
			Serverid = l.ServerId,
			Name = l.Name,
			Date = l.Date,
			Interval = l.Interval,
			CpuutilizationPercentage = l.CpuUsage,
			MemoryUtilizationPercentage = l.MemoryUsage
		};
	}
}