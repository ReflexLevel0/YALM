using System.Text;
using System.Text.Json;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.Logs;
using YALM.Monitor.Models;

namespace YALM.Monitor;

internal class Monitor
{
	private const string LastLogDateFilename = "log_date.txt";
	
	private static async Task Main()
	{
		var config = JsonSerializer.Deserialize<Config>(await File.ReadAllTextAsync("config.json"));
		if (config == null) throw new Exception("Configuration invalid!");
		var logHelper = new LogHelper(config, LastLogDateFilename);
		var apiHelper = new ApiHelper(config);
		
		while (true)
		{
			var log = await logHelper.Log();
			await apiHelper.SendLog(log);
		}
	}
}