using YALM.Common;

namespace YALM.API.GraphQL.Alerts;

public interface IAlertHelper
{
	Task RaiseAlert(int serverId, DateTimeOffset date, AlertSeverity severity, string text);
}