using LinqToDB;
using YALM.API.Db;
using YALM.API.Db.Models;
using YALM.Common;

namespace YALM.API.GraphQL.Alerts;

public class AlertHelper(IDbProvider dbProvider) : IAlertHelper
{
	public async Task RaiseAlert(int serverId, DateTimeOffset date, AlertSeverity severity, string text)
	{
		Console.WriteLine($"ALERT: {serverId} {date} {severity} {text}");
		await using var db = dbProvider.GetDb();
		
		try
		{
			await db.Alerts.InsertAsync(() => new AlertDbRecord { Serverid = serverId, Date = date, Text = text, Severity = (int)severity });
		}
		catch(Exception ex)
		{
			Console.WriteLine($"ERROR: {ex.Message}");
		}
	}
}