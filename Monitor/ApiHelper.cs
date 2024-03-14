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
    private readonly int _serverId;
    
    public ApiHelper(Config config, int serverId)
    {
        _config = config;
        _serverId = serverId;
    }
    
    public async Task SendLog(LogBase log)
    {
        //Generating query string
        var variables = new GraphqlVariables();
        var variableStringBuilder = new StringBuilder(256);
        var queryStringBuilder = new StringBuilder(1024);

        if (log.CpuInfo != null)
        {
            variableStringBuilder.Append("$cpu: CpuInput!,");
            queryStringBuilder.AppendLine("""
                                            addOrReplaceCpu(cpu: $cpu){
                                              error
                                            }
                                          """);
            variables.Cpu = new CpuInput(_serverId)
            {
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
                                            addCpuLog(log: $cpuLog){
                                              error
                                            }
                                          """);
            variables.CpuLog = new CpuLogInput
            {
                ServerId = _serverId,
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
                                            addMemoryLog(log: $memoryLog){
                                              error
                                            }
                                          """);
            variables.MemoryLog = new MemoryLogInput(_serverId)
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
                                            addProgramLogs(logs: $programLogs){
                                              error
                                            }
                                          """);
            variables.ProgramLogs = new List<ProgramLogInput>();
            foreach (var program in log.ProgramInfo.ProgramLogs)
            {
                var programLog = new ProgramLogInput
                {
                    ServerId = _serverId,
                    Date = log.LogTime,
                    Name = program.Name,
                    MemoryUsage = program.MemoryUsage,
                    CpuUsage = program.CpuUsage
                };
                variables.ProgramLogs.Add(programLog);
            }
        }
        
        if (log.Disks != null && log.Disks.Count != 0)
        {
        	variableStringBuilder.Append("$disks: [DiskInput!]!,");
            //$partitions: [PartitionInput]!, $partitionLogs: [PartitionLogsInput]!, 
        	queryStringBuilder.AppendLine("""
        	                            addOrReplaceDisks(disks: $disks){
        	                                error
        	                            }
        	                          """);
            
            // addPartitions(partitions: $partitions){
            //     error
            // },
            // addPartitionLogs(logs: $partitionLogs){
            //     error
            // },
            
        	variables.Disks = new List<DiskInput>();
            variables.Partitions = new List<PartitionInput>();
            foreach (var d in log.Disks)
            {
                var disk = new DiskInput(_serverId, d.DiskUuid, d.DiskType, d.Serial, d.Path, d.Vendor, d.Name, d.Bytes);
                variables.Disks.Add(disk);
                if (d.Children == null) continue;
                
                foreach(var p in d.Children)
                {
                    var partition = new PartitionInput(_serverId, p.DiskUuid, p.PartitionUuid, p.FilesystemType, p.FilesystemVersion, p.Label, p.Mountpoint);
                    variables.Partitions.Add(partition);
                }
            }
        
            //variables.PartitionLogs = new List<PartitionLogInput>();
        }

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