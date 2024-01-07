using System.Text;
using System.Text.Json;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Monitor.Models;

namespace YALM.Monitor;

internal class Monitor
{
	private static async Task Main()
	{
		var config = JsonSerializer.Deserialize<Config>(await File.ReadAllTextAsync("config.json"));
		if (config == null) throw new Exception("Configuration invalid!");
		var graphQlClient = new GraphQLHttpClient(config.ApiUrl, new NewtonsoftJsonSerializer());
		var logHelper = new LogHelper(config);

		while (true)
		{
			var log = await logHelper.Log();

			//Removing seconds from the date
			var date = DateTime.Now;
			date = date.AddSeconds(-date.Second);

			//Generating query string
			var variables = new GraphqlVariables();
			var variableStringBuilder = new StringBuilder(256);
			var queryStringBuilder = new StringBuilder(1024);

			if (log.CpuInfo != null)
			{
				variableStringBuilder.Append("$oldCpu: CpuIdInput!, $cpu: CpuInput!,");
				queryStringBuilder.Append("""
				                            updateCpu(oldCpuId: $oldCpu, newCpu: $cpu){
				                              error
				                            },
				                          """);
				variables.OldCpu = new CpuIdInput { ServerId = 0 };
				variables.Cpu = new CpuInput
				{
					ServerId = 0,
					Architecture = log.CpuInfo.Architecture,
					Name = log.CpuInfo.Name,
					FrequencyMhz = log.CpuInfo.Frequency,
					Threads = log.CpuInfo.Threads,
					Cores = log.CpuInfo.Cores
				};
			}
			
			// if (log.CpuLog != null)
			// {
			// 	variableStringBuilder.Append("$cpuLog: CpuLogInput!,");
			// 	queryStringBuilder.Append("""
			// 	                          addCpuLog(cpuLog: $cpuLog){
			// 	                              error
			// 	                          },
			// 	                          """);
			// 	variables.CpuLog = new CpuLogInput
			// 	{
			// 		ServerId = 0,
			// 		Interval = config.IntervalInMinutes,
			// 		Date = date,
			// 		Usage = log.CpuLog.Usage,
			// 		NumberOfTasks = log.CpuLog.NumberOfTasks
			// 	};
			// }

			// if (log.MemoryLog != null)
			// {
			// 	variableStringBuilder.Append("$memory: MemoryInput!,");
			// 	queryStringBuilder.Append("""
			// 	                          addMemoryLog(memory: $memory){
			// 	                              error
			// 	                          },
			// 	                          """);
			// 	variables.MemoryLog = new MemoryLogInput
			// 	{
			// 		ServerId = 0, Interval = config.IntervalInMinutes, Date = date, (int)log.MemoryLog.MemoryFreeKb, (int)log.MemoryLog.MemoryTotalKb
			// 	};
			// }
			//
			// if (log.StorageLogs != null && log.StorageLogs.Count != 0)
			// {
			// 	variableStringBuilder.Append("$storage: StorageLogInput!,");
			// 	queryStringBuilder.Append("""
			// 	                          addStorageLog(storage: $storage){
			// 	                              error
			// 	                          },
			// 	                          """);
			// 	variables.storage = new
			// 	{
			// 		serverId = 0,
			// 		date,
			// 		interval = config.IntervalInMinutes,
			// 		sorageVolumes = log.StorageLogs
			// 	};
			// }

			Console.WriteLine(log);

			//Configuring the request
			string requestString = $"mutation({variableStringBuilder}){{\n{queryStringBuilder}\n}}";
			Console.WriteLine(requestString);
			var request = new GraphQLRequest(requestString, variables: variables);

			try
			{
				//Sending the request and printing out result/errors
				var payload = await graphQlClient.SendMutationAsync<Payload<CpuLogInput>>(request);
				if (payload.Errors != null && payload.Errors.Length != 0) throw new Exception(payload.Errors[0].Message);
				if (payload.Data.Error != null) Console.WriteLine($"ERROR: {payload.Data.Error}");
				Console.WriteLine(payload.Data.Data);
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