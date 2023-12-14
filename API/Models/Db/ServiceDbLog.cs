using Common.Models;

namespace API.Models.Db;

public class ServiceDbLog : DbLogBase
{
	public int ServerId { get; }
	public string Name { get; }
	public int RamUsageMegabytes { get; }
	public ServiceStatus Status { get; }
	public int Tasks { get; }
	public double CpuSeconds { get; }

	public ServiceDbLog(int serverId, DateTime date, int interval, string name, int ramUsageMegabytes, ServiceStatus status, int tasks, double cpuSeconds) : base(date, interval)
	{
		ServerId = serverId;
		Name = name;
		RamUsageMegabytes = ramUsageMegabytes;
		Status = status;
		Tasks = tasks;
		CpuSeconds = cpuSeconds;
	}
}