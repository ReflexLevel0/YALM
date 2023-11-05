using System.Data.Common;
using System.Text;
using Common.Models.Graphql;
using Common.Models.Graphql.InputModels;
using Common.Models.Graphql.OutputModels;
using Npgsql;

namespace API;

public class Mutation
{
	private readonly IDb _db;

	public Mutation(IDb db)
	{
		_db = db;
	}

	/// <summary>
	/// Used for adding a generic log to the database 
	/// </summary>
	/// <param name="readerToObject">Converts database row to object of type <see cref="TReader"/></param>
	/// <param name="objectsToOutput">Converts object of type <see cref="TReader"/> into <see cref="TOutput"/></param>
	/// <param name="insertQuery">SQL query used for inserting the log into the database</param>
	/// <param name="fetchQuery">SQL query for fetching the inserted log from the database</param>
	/// <typeparam name="TInput">Type of record that should be inserted into the database</typeparam>
	/// <typeparam name="TReader">Middleware object type</typeparam>
	/// <typeparam name="TOutput">Type of record that is returned to the user</typeparam>
	/// <returns></returns>
	private async Task<Payload<TOutput>> AddLog<TInput, TReader, TOutput>(Func<NpgsqlDataReader, TReader> readerToObject, Func<List<TReader>, TOutput> objectsToOutput, string insertQuery, string fetchQuery) where TOutput : GraphqlModelBase
	{
		var objects = new List<TReader>();
		
		try
		{
			//Inserting the log into the database
			await _db.ExecuteNonQueryAsync(insertQuery);

			//Reading the log that was just added so it can be returned
			await foreach (var reader in _db.ExecuteReaderAsync(fetchQuery))
			{
				objects.Add(readerToObject(reader));
			}
		}
		catch (DbException ex)
		{
			return new Payload<TOutput> { Error = ex.ToString() };
		}

		var log = objectsToOutput(objects);
		return new Payload<TOutput> { Log = log };
	}

	public async Task<Payload<CpuOutput>> AddCpuLog(CpuInput cpu)
	{
		string date = DateToString(cpu.Date);
		var payload = await AddLog<CpuInput, CpuInput, CpuOutput>(reader =>
			{
				var log = _db.ParseCpuRecord(reader);
				return new CpuInput(log.ServerId, log.Date, log.Interval, log.Usage, log.NumberOfTasks);
			},
			objects =>
			{
				if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
				var obj = objects.First();
				return new CpuOutput(obj.ServerId, obj.Date, obj.Usage, obj.NumberOfTasks);
			},
			$"INSERT INTO Cpu(serverId, date, interval, usage, numberoftasks) " +
			$"VALUES ({cpu.ServerId}, '{date}', {cpu.Interval}, {cpu.Usage}, {cpu.NumberOfTasks})",
			$"SELECT serverid, date, interval, usage, numberoftasks " +
			$"FROM Cpu WHERE serverId = {cpu.ServerId} AND date = '{date}'");
		return payload;
	}

	public async Task<Payload<MemoryOutput>> AddMemoryLog(MemoryInput memory)
	{
		string date = DateToString(memory.Date);
		var payload = await AddLog<MemoryInput, MemoryInput, MemoryOutput>(reader =>
			{
				var log = _db.ParseMemoryRecord(reader);
				return new MemoryInput(log.ServerId, log.Date, log.Interval, log.MbUsed, log.MbTotal);
			},
			objects =>
			{
				if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
				var obj = objects.First();
				return new MemoryOutput(obj.ServerId, obj.Date, obj.MbUsed, obj.MbTotal);
			},
			$"INSERT INTO Memory(serverId, date, interval, mbUsed, mbTotal)" +
			$"VALUES({memory.ServerId}, '{date}', {memory.Interval}, {memory.MbUsed}, {memory.MbTotal})",
			$"SELECT serverid, date, interval, mbUsed, mbTotal " +
			$"FROM Memory WHERE serverId = {memory.ServerId} AND date = '{date}'");
		return payload;
	}

	public async Task<Payload<StorageOutput>> AddStorageLog(StorageInput storage)
	{
		string date = DateToString(storage.Date);
		var updatePartitionsSqlQuery = new StringBuilder(1024);
		var insertLogsSqlQuery = new StringBuilder(1024);
		foreach (var volume in storage.Volumes)
		{
			Console.WriteLine(updatePartitionsSqlQuery.Length);
			if (updatePartitionsSqlQuery.Length != 0) updatePartitionsSqlQuery.Append(',');
			updatePartitionsSqlQuery.Append($"('{volume.UUID}', '{volume.FilesystemName}', '{volume.FilesystemVersion}', '{volume.Label}')");
			if (insertLogsSqlQuery.Length != 0) insertLogsSqlQuery.Append(',');
			insertLogsSqlQuery.Append($"({storage.ServerId}, '{date}', '{volume.UUID}', {storage.Interval}, {volume.Bytes}, {volume.UsedPercentage}, '{volume.MountPath}')");
		}
		updatePartitionsSqlQuery.Append(" ON CONFLICT(uuid) DO UPDATE SET filesystemName=EXCLUDED.filesystemName, filesystemVersion=Excluded.filesystemVersion, label = EXCLUDED.label; ");
		
		var payload = await AddLog<StorageInput, StorageVolume, StorageOutput>(reader =>
			{
				var log = _db.ParseStorageRecord(reader);
				return new StorageVolume(log.UUID, log.Label, log.FilesystemName, log.FilesystemVersion, log.MountPath, log.Bytes, log.UsedPercentage);
			},
			objects => new StorageOutput(storage.ServerId, storage.Date, objects),
			$"INSERT INTO partition(uuid, filesystemName, filesystemVersion, label) " +
			$"VALUES {updatePartitionsSqlQuery} " +
			$"INSERT INTO storagelog(serverid, date, uuid, interval, bytestotal, usage, mountpath) " +
			$"VALUES {insertLogsSqlQuery}",
			$"SELECT serverid, date, interval, partition.uuid, label, filesystemname, filesystemversion, mountpath, bytesTotal, usage " +
			$"FROM storagelog JOIN partition ON storagelog.uuid = partition.uuid WHERE serverid = {storage.ServerId} AND date = '{date}'");
		return payload;
	}
	
	private static string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}