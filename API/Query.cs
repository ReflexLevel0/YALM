using API.Models;

namespace API;

public class Query
{
	private readonly Db _db;
	
	public Query(Db db)
	{
		_db = db;
	}
	
	public async IAsyncEnumerable<Cpu> Cpu(string startDateString, string endDateString)
	{
		await foreach (var reader in _db.ExecuteReadAsync(
			               $"SELECT serverid, date, interval, usage, numberoftasks " +
			               $"FROM Cpu {CreateSqlFromStartAndEndDate(startDateString, endDateString)}"))
		{
			int id = reader.GetInt32(0);
			var date = reader.GetDateTime(1);
			int interval = reader.GetInt32(2);
			double usage = reader.GetDouble(3);
			int numberOfTasks = reader.GetInt32(4);
			yield return new Cpu(id, date, interval, usage, numberOfTasks);
		}
	}

	public Ram Ram()
	{
		return new Ram(0, DateTime.Today, 5, 1512, 8192);
	}

	private string CreateSqlFromStartAndEndDate(string startDate, string endDate)
	{
		string result = "WHERE ";
		bool startDateSpecified = DateTime.TryParse(startDate, out _);
		if (startDateSpecified) result += $"date>='{startDate}'";
		bool endDateSpecified = DateTime.TryParse(endDate, out _);
		if (startDateSpecified && endDateSpecified) result += $" AND date<='{endDate}'";
		else if (endDateSpecified) result += $"date<='{endDate}'";
		return result;
	}
}