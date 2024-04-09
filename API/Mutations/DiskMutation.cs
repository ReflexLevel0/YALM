using DataModel;
using HotChocolate.Language;
using YALM.API.Db.Models;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class DiskMutation(IDb db, IMutationHelper mutationHelper)
{
	private readonly Func<DiskIdInput, IQueryable<DiskDbRecord>> _getDiskQuery = disk =>
		from d in db.Disks
		where d.ServerId == disk.ServerId && string.CompareOrdinal(d.Uuid, disk.Uuid) == 0
		select d;

	private readonly Func<DiskDbRecord, DiskIdInput> _getDiskId = disk => new DiskIdInput(disk.ServerId, disk.Uuid);
	
	public async Task<Payload<DiskOutputBase>> AddDisk(DiskInput disk)
	{
		var model = InputToDbModel(disk);
		return await mutationHelper.AddModelAsync<DiskIdInput, DiskDbRecord, DiskOutputBase>(model, _getDiskId(model), _getDiskQuery);
	}

	public async Task<Payload<DiskOutputBase>> AddOrReplaceDisk(DiskInput disk)
	{
		var model = InputToDbModel(disk);
		return await mutationHelper.AddOrReplaceModelAsync<DiskIdInput, DiskDbRecord, DiskOutputBase>(model, _getDiskId(model), _getDiskQuery);
	}
	
	public async Task<Payload<List<DiskOutputBase>>> AddOrReplaceDisks(List<DiskInput> disks)
	{
		var diskModels = new List<DiskDbRecord>();
		disks.ForEach(d => diskModels.Add(InputToDbModel(d)));
		return await mutationHelper.AddOrReplaceModelsAsync<DiskIdInput, DiskDbRecord, DiskOutputBase>(diskModels, _getDiskId, _getDiskQuery);
	}

	public async Task<Payload<DiskOutputBase>> DeleteDisk(DiskIdInput diskId)
	{
		return await mutationHelper.DeleteModelAsync<DiskIdInput, DiskDbRecord, DiskOutputBase>(diskId, _getDiskQuery);
	}
	
	private static DiskDbRecord InputToDbModel(DiskInput disk)
	{
		var dbModel = new DiskDbRecord
		{
			Uuid = disk.Uuid,
			ServerId = disk.ServerId,
			BytesTotal = disk.BytesTotal,
			Model = disk.Model,
			Path = disk.Path,
			Serial = disk.Serial,
			Type = disk.Type,
			Vendor = disk.Vendor
		};

		return dbModel;
	}
}