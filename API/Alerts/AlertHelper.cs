using LinqToDB;
using YALM.API.Db;
using YALM.API.Db.Models;
using YALM.Common;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Alerts;

public class AlertHelper(IDbProvider dbProvider) : IAlertHelper
{
	public async Task RaiseAlert(int serverId, DateTime date, AlertSeverity severity, string text)
	{
		Console.WriteLine($"ALERT: {serverId} {date} {severity} {text}");
		await using var db = dbProvider.GetDb();
		string modifiedText = severity switch
		{
			AlertSeverity.Information => "[INFO]",
			AlertSeverity.Warning => "[WARNING]",
			AlertSeverity.Critical => "[CRITICAL]",
			_ => throw new NotImplementedException()
		};
		modifiedText += $" {text}";
		await db.Alerts.InsertAsync(() => new AlertDbRecord{Serverid = serverId, Date = date, Text = modifiedText});
	}
}