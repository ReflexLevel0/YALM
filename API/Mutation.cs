using System.Data.Common;
using API.Models.Db;
using Common.Models.Graphql;
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
	/// <param name="readerToGraphqlObject">Method used to convert <see cref="NpgsqlDataReader"/> record to a GraphQL object which is ready to be returned to the user</param>
	/// <param name="insertQuery">SQL query used for inserting the log into the database</param>
	/// <param name="fetchQuery">SQL query for fetching the inserted log from the database</param>
	/// <typeparam name="TModel">Log that was inserted into the database</typeparam>
	/// <returns></returns>
	private async Task<Payload<TModel>> AddLog<TModel>(Func<NpgsqlDataReader, TModel> readerToGraphqlObject, string insertQuery, string fetchQuery) where TModel : GraphqlModelBase
	{
		TModel? log = null;

		try
		{
			//Inserting the log into the database
			await _db.ExecuteNonQueryAsync(insertQuery);

			//Reading the log that was just added so it can be returned
			await foreach (var reader in _db.ExecuteReaderAsync(fetchQuery))
			{
				log = readerToGraphqlObject(reader);
				break;
			}
		}
		catch (DbException ex)
		{
			return new Payload<TModel> { Error = ex.ToString() };
		}

		return new Payload<TModel> { Log = log };
	}

	public async Task<Payload<Cpu>> AddCpuLog(CpuLog cpu)
	{
		string date = DateToString(cpu.Date);
		var payload = await AddLog<Cpu>(reader =>
			{
				var log = _db.ParseCpuRecord(reader);
				return new Cpu(log.ServerId, log.Date, log.Usage, log.NumberOfTasks);
			},
			$"INSERT INTO Cpu(serverId, date, interval, usage, numberoftasks) " +
			$"VALUES ({cpu.ServerId}, '{date}', {cpu.Interval}, {cpu.Usage}, {cpu.NumberOfTasks})",
			$"SELECT serverid, date, interval, usage, numberoftasks " +
			$"FROM Cpu WHERE serverId = {cpu.ServerId} AND date = '{date}'");
		return payload;
	}

	public async Task<Payload<Memory>> AddMemoryLog(MemoryLog memory)
	{
		string date = DateToString(memory.Date);
		var payload = await AddLog<Memory>(reader =>
			{
				var log = _db.ParseMemoryRecord(reader);
				return new Memory(log.ServerId, log.Date, log.MbUsed, log.MbTotal);
			},
			$"INSERT INTO Memory(serverId, date, interval, mbUsed, mbTotal)" +
			$"VALUES({memory.ServerId}, '{date}', {memory.Interval}, {memory.MbUsed}, {memory.MbTotal})",
			$"SELECT serverid, date, interval, mbUsed, mbTotal " +
			$"FROM Memory WHERE serverId = {memory.ServerId} AND date = '{date}'");
		return payload;
	}

	private static string DateToString(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
}