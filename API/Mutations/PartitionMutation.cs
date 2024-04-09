using DataModel;
using HotChocolate.Language;
using YALM.API.Db.Models;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class PartitionMutation(IDb db, IMutationHelper mutationHelper)
{
	private readonly Func<PartitionIdInput, IQueryable<PartitionDbRecord>> _getPartitionQuery = pId =>
		from p in db.Partitions
		where pId.ServerId == p.Serverid && string.CompareOrdinal(pId.Uuid, p.Uuid) == 0
		select p;

	private readonly Func<PartitionDbRecord, PartitionIdInput> _getPartitionId = p => new PartitionIdInput(p.Serverid, p.Uuid);

	public async Task<Payload<PartitionOutputBase>> AddOrReplacePartition(PartitionInput partition)
	{
		var model = InputToDbModel(partition);
		return await mutationHelper.AddOrReplaceModelAsync<PartitionIdInput, PartitionDbRecord, PartitionOutputBase>(model, _getPartitionId(model), _getPartitionQuery);
	}

	public async Task<Payload<List<PartitionOutputBase>>> AddOrReplacePartitions(List<PartitionInput> partitions)
	{
		var partitionDbRecords = new List<PartitionDbRecord>();
		partitions.ForEach(p => partitionDbRecords.Add(InputToDbModel(p)));
		return await mutationHelper.AddOrReplaceModelsAsync<PartitionIdInput, PartitionDbRecord, PartitionOutputBase>(partitionDbRecords, _getPartitionId, _getPartitionQuery);
	}

	private static PartitionDbRecord InputToDbModel(PartitionInput partition)
	{
		return new PartitionDbRecord
		{
			Serverid = partition.ServerId,
			Diskuuid = partition.DiskUuid,
			Uuid = partition.Uuid,
			Label = partition.PartitionLabel,
			FilesystemName = partition.FilesystemName,
			FilesystemVersion = partition.FilesystemVersion,
			MountPath = partition.Mountpath
		};
	}
}