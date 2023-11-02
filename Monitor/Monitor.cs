using System.Text;
using System.Text.Json;
using Common.Models.Graphql;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Monitor.Models;

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

			//Generating query string
			var variables = new GraphqlVariables();
			var variableStringBuilder = new StringBuilder(256);
			var queryStringBuilder = new StringBuilder(1024);
			if (log.CpuLog != null)
			{
				variableStringBuilder.Append("$cpu: CpuLogInput!,");
				queryStringBuilder.Append("""
				                          addCpuLog(cpu: $cpu){
				                              error
				                          },
				                          """);
				variables.cpu = new
				{
					serverId = 0,
					date,
					interval = config.IntervalInMinutes,
					numberOfTasks = log.CpuLog?.NumberOfTasks,
					usage = log.CpuLog?.Usage
				};
			}

			// if (log.MemoryLog != null)
			// {
			// 	variableStringBuilder.Append("$memory: MemoryLogInput!,");
			// 	queryStringBuilder.Append("""
			// 	                          addMemoryLog(memory: $memory){
			// 	                          		error
			// 	                          },
			// 	                          """);
			// 	variables.memory = new
			// 	{
			// 		serverId = 0,
			// 		date,
			// 		interval = config.IntervalInMinutes,
			// 		usedMemoryMb = log.MemoryLog?.UsedMemoryMb,
			// 		totalMemoryMb = log.MemoryLog?.TotalMemoryMb
			// 	};
			// }

			//Configuring the request
			var request = new GraphQLRequest(
				$"mutation({variableStringBuilder}){{{queryStringBuilder}}}",
				variables: variables
			);

			try
			{
				//Sending the request and printing out result/errors
				var payload = await graphQlClient.SendMutationAsync<Payload<Cpu>>(request);
				if (payload.Errors != null && payload.Errors.Length != 0) throw new Exception(payload.Errors[0].Message);
				if (payload.Data.Error != null) Console.WriteLine($"ERROR: {payload.Data.Error}");
				Console.WriteLine(payload.Data.Log);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in sending POST request to server: {ex.Message}");
			}

			//TODO: this could cause issues since it doesnt take into account processing time
			Thread.Sleep(config.IntervalInMinutes * 60 * 1000);
		}
	}
}