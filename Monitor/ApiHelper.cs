using System.Text;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Monitor.Models;
using YALM.Monitor.Models.LogInfo;

namespace YALM.Monitor;

public class ApiHelper
{
    private readonly Config _config;
    
    public ApiHelper(Config config)
    {
        _config = config;
    }
    
    public async Task SendLog(LogBase log)
    {
        //Generating query string
        var variables = new GraphqlVariables();
        var variableStringBuilder = new StringBuilder(256);
        var queryStringBuilder = new StringBuilder(1024);

        if (log.CpuInfo != null)
        {
            variableStringBuilder.Append("$oldCpu: CpuIdInput!, $cpu: CpuInput!,");
            queryStringBuilder.AppendLine("""
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

        if (log.ProgramInfo?.CpuLog != null)
        {
            variableStringBuilder.Append("$cpuLog: CpuLogInput!, ");
            queryStringBuilder.AppendLine("""
                                            addCpuLog(cpuLog: $cpuLog){
                                              error
                                            }
                                          """);
            variables.CpuLog = new CpuLogInput
            {
                ServerId = 0,
                NumberOfTasks = log.ProgramInfo.CpuLog.NumberOfTasks,
                Interval = _config.IntervalInMinutes,
                Date = log.LogTime,
                Usage = log.ProgramInfo.CpuLog.Usage
            };
        }

        if (log.ProgramInfo?.MemoryLog != null)
        {
            variableStringBuilder.Append("$memoryLog: MemoryLogInput!, ");
            queryStringBuilder.AppendLine("""
                                            addMemoryLog(memoryLog: $memoryLog){
                                              error
                                            }
                                          """);
            variables.MemoryLog = new MemoryLogInput(0)
            {
                Interval = _config.IntervalInMinutes,
                Date = log.LogTime,
                TotalKb = log.ProgramInfo.MemoryLog.MemoryTotalKb,
                FreeKb = log.ProgramInfo.MemoryLog.MemoryFreeKb,
                UsedKb = log.ProgramInfo.MemoryLog.MemoryUsedKb,
                SwapTotalKb = log.ProgramInfo.MemoryLog.SwapTotalKb,
                SwapFreeKb = log.ProgramInfo.MemoryLog.SwapFreeKb,
                SwapUsedKb = log.ProgramInfo.MemoryLog.SwapUsedKb,
                CachedKb = log.ProgramInfo.MemoryLog.CachedKb,
                AvailableKb = log.ProgramInfo.MemoryLog.AvailableMemoryKb
            };
        }

        if (log.ProgramInfo?.ProgramLogs != null)
        {
            variableStringBuilder.Append("$programLogs: [ProgramLogInput!]!, ");
            queryStringBuilder.AppendLine("""
                                            addProgramLogs(programLogs: $programLogs){
                                              error
                                            }
                                          """);
            variables.ProgramLogs = new List<ProgramLogInput>();
            foreach (var program in log.ProgramInfo.ProgramLogs)
            {
                var programLog = new ProgramLogInput
                {
                    ServerId = 0,
                    Date = log.LogTime,
                    Name = program.Name,
                    MemoryUsage = program.MemoryUsage,
                    CpuUsage = program.CpuUsage
                };
                variables.ProgramLogs.Add(programLog);
            }
        }

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
        // 		storageVolumes = log.StorageLogs
        // 	};
        // }

        Console.WriteLine(log);

        //Configuring the request
        string requestString = $"mutation({variableStringBuilder}){{\n{queryStringBuilder}}}";
        Console.WriteLine(requestString);
        var request = new GraphQLRequest(requestString, variables: variables);

        try
        {
            //Sending the request and printing out result/errors
            var graphQlClient = new GraphQLHttpClient(_config.ApiUrl, new NewtonsoftJsonSerializer());
            var payload = await graphQlClient.SendMutationAsync<Payload<CpuLogInput>>(request);
            if (payload.Errors != null && payload.Errors.Length != 0) throw new Exception(payload.Errors[0].Message);
            if (payload.Data.Error != null) Console.WriteLine($"ERROR: {payload.Data.Error}");
            Console.WriteLine(payload.Data.Data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in sending POST request to server: {ex.Message}");
        }
    }
}