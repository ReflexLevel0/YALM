using System.Data.Common;
using API.Models.Db;
using API.Models.Graphql;

namespace API.Models;

public class Mutation
{
	private readonly API.Db _db;

	public Mutation(API.Db db)
	{
		_db = db;
	}

	public async Task<CpuAddedPayload> AddCpuLog(CpuLog cpu)
	{
		CpuLog? log = null;
		try
		{
			Console.WriteLine(cpu.Date);
			await _db.ExecuteNonQueryAsync($"INSERT INTO Cpu(serverId, date, interval, usage, numberoftasks) " +
			                               $"VALUES ({cpu.ServerId}, '{cpu.Date:yyyy-MM-dd HH:mm:ss}', {cpu.Interval}, {cpu.Usage}, {cpu.NumberOfTasks})");
			string q = $"SELECT serverid, date, interval, usage, numberoftasks " +
			           $"FROM Cpu WHERE serverId = {cpu.ServerId} AND " +
			           $"date = '{cpu.Date:yyyy-MM-dd HH:mm:ss zz}'";
			Console.WriteLine(q);
			await foreach (var reader in _db.ExecuteReaderAsync
				               (q))
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