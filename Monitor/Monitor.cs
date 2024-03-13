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
	private const string ServerIdFilename = "server_id.txt";
	
	private static async Task Main()
	{
		var config = JsonSerializer.Deserialize<Config>(await File.ReadAllTextAsync("config.json"));
		if (config == null) throw new Exception("Configuration invalid!");

		//Creating server ID if the program is running for the first time
		if (File.Exists(ServerIdFilename) == false)
		{
			await File.WriteAllTextAsync(ServerIdFilename, DateTime.UtcNow.GetHashCode().ToString());
		}

		int serverId = int.Parse(ServerIdFilename);
		var logHelper = new LogHelper(config, LastLogDateFilename);
		var apiHelper = new ApiHelper(config, serverId);
        
		while (true)
		{
			var log = await logHelper.Log();
			await apiHelper.SendLog(log);
		}
	}
}