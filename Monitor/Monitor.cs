using System.Text.Json;
using Common.Models.Graphql;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Monitor;

internal class Monitor
{
	private static async Task Main()
	{
		var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
		if (config == null) throw new Exception("Configuration invalid!");
		var graphQlClient = new GraphQLHttpClient(config.ApiUrl, new NewtonsoftJsonSerializer());
		var logHelper = new LogHelper(config);

		while (true)
		{
			var log = logHelper.Log();

			//Removing seconds from the date
			var date = DateTime.Now;
			date = date.AddSeconds(-date.Second);
			
			//Configuring the request
			var request = new GraphQLRequest(
				"""
				mutation($cpu: CpuLogInput!){
				    addCpuLog(cpu: $cpu){
				        error
				    }
				}
				""",
				variables: new
				{
					cpu = new
					{
						serverId = 0,
						date = date,
						interval = config.IntervalInMinutes,
						numberOfTasks = log.CpuLog?.NumberOfTasks,
						usage = log.CpuLog?.Usage
					}
				});

			try
			{
				//Sending the request and printing out result/errors
				var payload = await graphQlClient.SendMutationAsync<CpuAddedPayload>(request);
				if (payload.Errors != null && payload.Errors.Length != 0) throw new Exception(payload.Errors[0].Message);
				if (payload.Data.Error != null) Console.WriteLine($"ERROR: {payload.Data.Error}");
				Console.WriteLine(payload.Data.Cpu);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in sending POST request to server: {ex.Message}");
			}

			Thread.Sleep(config.IntervalInMinutes * 60 * 1000);
		}
	}
}