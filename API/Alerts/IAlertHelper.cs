using YALM.Common;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Alerts;

public interface IAlertHelper
{
	Task RaiseAlert(int serverId, DateTime date, AlertSeverity severity, string text);
}