using System.Data.Common;
using Common.Models.Graphql;
using Common.Models.Graphql.InputModels;
using Common.Models.Graphql.Logs;
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
	private async Task<Payload<TOutput>> ExecuteInsertQuery<TInput, TReader, TOutput>(Func<NpgsqlDataReader, TReader> readerToObject, Func<List<TReader>, TOutput> objectsToOutput, string insertQuery, string fetchQuery)
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
		return new Payload<TOutput> { Data = log };
	}

	public async Task<Payload<CpuLog>> AddCpuLog(CpuLogInput cpuLog)
	{
		string date = DateToString(cpuLog.Date);
		var payload = await ExecuteInsertQuery<CpuLogInput, CpuLogInput, CpuLog>(
			reader =>
			{
				var log = _db.ParseCpuLogRecord(reader);
				return new CpuLogInput(log.ServerId, log.Interval, log.Date, log.Usage, log.NumberOfTasks);
			},
			objects =>
			{
				if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
				var obj = objects.First();
				return new CpuLog(obj.Date, obj.Usage, obj.NumberOfTasks);
			},
			$"INSERT INTO Cpu(serverId, date, interval, usage, numberoftasks) " +
			$"VALUES ({cpuLog.ServerId}, '{date}', {cpuLog.Interval}, {cpuLog.Usage}, {cpuLog.NumberOfTasks})",
			$"SELECT serverid, date, interval, usage, numberoftasks " +
			$"FROM Cpu WHERE serverId = {cpuLog.ServerId} AND date = '{date}'");
		return payload;
	}
	
	public async Task<Payload<MemoryLog>> AddMemoryLog(MemoryLogInput memoryLog)
	{
		string date = DateToString(memoryLog.Date);
		var payload = await ExecuteInsertQuery<MemoryLogInput, MemoryLogInput, MemoryLog>(
			reader =>
			{
				var log = _db.ParseMemoryLogRecord(reader);
				return new MemoryLogInput(log.ServerId, log.Interval, log.Date, log.MbUsed, log.MbTotal);
			},
			objects =>
			{
				if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
				var obj = objects.First();
				return new MemoryLog(obj.Date, obj.MbUsed, obj.MbTotal);
			},
			$"INSERT INTO Memory(serverId, date, interval, mbUsed, mbTotal)" +
			$"VALUES({memoryLog.ServerId}, '{date}', {memoryLog.Interval}, {memoryLog.MbUsed}, {memoryLog.MbTotal})",
			$"SELECT serverid, date, interval, mbUsed, mbTotal " +
			$"FROM Memory WHERE serverId = {memoryLog.ServerId} AND date = '{date}'");
		return payload;
	}
	
	public async Task<Payload<PartitionLog>> AddPartitionLog(PartitionLogInput partitionLog)
	{
		string date = DateToString(partitionLog.Date);
		var payload = await ExecuteInsertQuery<PartitionLogInput, PartitionLogInput, PartitionLog>(
			reader =>
			{
				var log = _db.ParsePartitionLogRecord(reader);
				return new PartitionLogInput(log.ServerId, log.Date, log.Interval, log.UUID, log.Bytes, log.UsedPercentage);
			},
			objects =>
			{
				if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
				var obj = objects.First();
				return new PartitionLog(obj.Date, obj.Bytes, obj.UsedPercentage);
			},
			"INSERT INTO partitionlog(serverid, uuid, date, interval, bytestotal, usage) " + 
			$"VALUES({partitionLog.ServerId}, '{partitionLog.Uuid}', '{date}', {partitionLog.Interval}, {partitionLog.Bytes}, {partitionLog.UsedPercentage})",
			"SELECT serverId, uuid, date, interval, bytestotal, usage " + 
			$"FROM partitionlog WHERE serverid = {partitionLog.ServerId} AND uuid = '{partitionLog.Uuid}' AND date = '{date}'");
		return payload;
	}
	
	public async Task<Payload<PartitionOutputBase>> AddPartition(PartitionInput partition)
	{
		//Getting disk id for the specified disk label
		int? diskId = null;
		await foreach (var reader in _db.ExecuteReaderAsync($"SELECT id FROM disk WHERE label = '{partition.DiskLabel}'"))
		{
			diskId = reader.GetInt32(0);
			break;
		}
		
		if (diskId == null) throw new Exception("Error in parsing disk data!");
		
		//Adding partition to the database
		var payload = await ExecuteInsertQuery<PartitionInput, PartitionInput, PartitionOutputBase>(
			reader =>
			{
				var log = _db.ParsePartitionRecord(reader);
				return new PartitionInput(partition.DiskLabel, log.Uuid, log.FilesystemName, log.FilesystemVersion, log.Label, log.MountPath);
			},
			objects =>
			{
				if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
				var obj = objects.First();
				return new PartitionOutputBase(obj.Uuid, obj.FilesystemName, obj.FilesystemVersion, obj.PartitionLabel, obj.Mountpath);
			},
			"INSERT INTO partition(diskid, uuid, filesystemname, filesystemversion, label, mountpath) " + 
			$"VALUES({diskId}, '{partition.Uuid}', '{partition.FilesystemName}', '{partition.FilesystemVersion}', '{partition.PartitionLabel}', '{partition.Mountpath}')",
			"SELECT uuid, filesystemname, filesystemversion, label, mountpath " + 
			$"FROM partition"
		);
		return payload;
	}
	
	public async Task<Payload<DiskOutputBase>> AddDisk(DiskInput disk)
	{
		var payload = await ExecuteInsertQuery<DiskInput, DiskOutputBase, DiskOutputBase>(
			reader =>
			{
				var log = _db.ParseDiskRecord(reader);
				return new DiskOutputBase(log.ServerId, log.Label);
			},
			objects =>
			{
				if (objects.Count != 1) throw new Exception($"Error: reader count is {objects.Count}");
				return objects.First();
			},
			"INSERT INTO disk(serverid, label) " + 
			$"VALUES({disk.ServerId}, '{disk.Label}')",
			"SELECT serverId, label " + 
			$"FROM disk WHERE serverid = {disk.ServerId} AND label = '{disk.Label}'");
		return payload;
	}
	
	private static string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}