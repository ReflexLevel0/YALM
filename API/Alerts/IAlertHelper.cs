using YALM.Common;

namespace YALM.API.Alerts;

public interface IAlertHelper
{
	Task RaiseAlert(int serverId, DateTimeOffset date, AlertSeverity severity, string text);
}