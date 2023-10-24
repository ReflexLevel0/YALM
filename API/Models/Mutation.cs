using System.Data.Common;
using API.Models.Db;
using Common.Models.Graphql;

namespace API.Models;

public class Mutation
{
	private readonly IDb _db;

	public Mutation(IDb db)
	{
		_db = db;
	}
	
	public async Task<CpuAddedPayload> AddCpuLog(CpuLog cpu)
	{
		CpuLog? log = null;
		try
		{
			//Inserting the CPU into the database
			await _db.ExecuteNonQueryAsync($"INSERT INTO Cpu(serverId, date, interval, usage, numberoftasks) " +
			                               $"VALUES ({cpu.ServerId}, '{cpu.Date:yyyy-MM-dd HH:mm:ss}', " +
			                               $"{cpu.Interval}, {cpu.Usage}, {cpu.NumberOfTasks})");
			
			//Reading the CPU log that was just added so it can be returned
			string query = $"SELECT serverid, date, interval, usage, numberoftasks " + 
			               $"FROM Cpu WHERE serverId = {cpu.ServerId} AND " + 
			               $"date = '{cpu.Date:yyyy-MM-dd HH:mm:ss}'";
			await foreach (var reader in _db.ExecuteReaderAsync(query))
			{
				log = _db.ParseCpuRecord(reader);
				break;
			}
		}
		catch (DbException ex)
		{
			return new CpuAddedPayload(null, ex.ToString());
		}

		return new CpuAddedPayload(log == null ? null : new Cpu(log.ServerId, log.Date, log.Usage, log.NumberOfTasks), null);
	}
}