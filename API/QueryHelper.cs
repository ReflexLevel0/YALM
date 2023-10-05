using System.Text;

namespace API;

public class QueryHelper
{
	public static string LimitSqlByParameters(int? serverId, string? startDateTime, string? endDateTime)
	{
		var result = new StringBuilder(256);
		
		if (serverId != null) result.Append($"serverId = {serverId}");

		if (startDateTime != null)
		{
			if (result.Length != 0) result.Append(" AND ");
			result.Append($"date >= '{startDateTime}'");
		}

		if (endDateTime != null)
		{
			if (result.Length != 0) result.Append(" AND ");
			result.Append($"date <= '{endDateTime}'");
		}

		return result.ToString();
	}

	public static double CombineValues(string method, IEnumerable<double> values)
	{
		double result;
		switch (method)
		{
			case "avg":
			case "average":
				result = values.Average();
				break;
			case "min":
				result = values.Min();
				break;
			case "max":
				result = values.Max();
				break;
			default:
				throw new Exception($"Invalid method '{method}'");
		}

		return result;
	}
}