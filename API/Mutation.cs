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
	/// <param name="readerToLogMethod">Method used to convert <see cref="NpgsqlDataReader"/> record to <see cref="TLog"/></param>
	/// <param name="insertQuery">SQL query used for inserting the log into the database</param>
	/// <param name="fetchQuery">SQL query for fetching the inserted log from the database</param>
	/// <typeparam name="TLog">Log that was inserted into the database</typeparam>
	/// <returns></returns>
	private async Task<TLog?> AddLog<TLog>(Func<NpgsqlDataReader, TLog> readerToLogMethod, string insertQuery, string fetchQuery) where TLog : LogBase
	{
		TLog? log = null;

		//Inserting the log into the database
		await _db.ExecuteNonQueryAsync(insertQuery);

		//Reading the log that was just added so it can be returned
		await foreach (var reader in _db.ExecuteReaderAsync(fetchQuery))
		{
			log = readerToLogMethod(reader);
			break;
		}

		return log;
	}

	public async Task<CpuAddedPayload> AddCpuLog(CpuLog cpu)
	{
		CpuLog? log;
		try
		{
			log = await AddLog<CpuLog>(reader => _db.ParseCpuRecord(reader),
				$"INSERT INTO Cpu(serverId, date, interval, usage, numberoftasks) " +
				$"VALUES ({cpu.ServerId}, '{cpu.Date:yyyy-MM-dd HH:mm:ss}', " +
				$"{cpu.Interval}, {cpu.Usage}, {cpu.NumberOfTasks})",
				$"SELECT serverid, date, interval, usage, numberoftasks " +
				$"FROM Cpu WHERE serverId = {cpu.ServerId} AND " +
				$"date = '{cpu.Date:yyyy-MM-dd HH:mm:ss}'");
		}
		catch (DbException ex)
		{
			return new CpuAddedPayload(null, ex.ToString());
		}

		return new CpuAddedPayload(log == null ? null : new Cpu(log.ServerId, log.Date, log.Usage, log.NumberOfTasks), null);
	}
}