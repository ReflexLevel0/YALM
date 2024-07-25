using DataModel;
using LinqToDB;
using YALM.API.Db.Models;

namespace YALM.API.Db;

public class MonitoringDbProvider : IDbProvider
{
	public IDb GetDb()
	{
		string connectionString = File.ReadAllText("dbConnectionString.txt");
		var dataOptions = new DataOptions<MonitoringDb>(new DataOptions().UsePostgreSQL(connectionString));
		return new MonitoringDb(dataOptions);
	}
}