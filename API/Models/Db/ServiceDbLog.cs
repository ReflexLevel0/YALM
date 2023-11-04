using Common.Models;

namespace API.Models.Db;

public class ServiceDbLog : DbLogBase
{
	public string Name { get; }
	public int RamUsageMegabytes { get; }
	public ServiceStatus Status { get; }
	public int Tasks { get; }
	public double CpuSeconds { get; }

	public ServiceDbLog(int serverId, DateTime date, int interval, string name, int ramUsageMegabytes, ServiceStatus status, int tasks, double cpuSeconds) : base(serverId, date, interval)
	{
		Name = name;
		RamUsageMegabytes = ramUsageMegabytes;
		Status = status;
		Tasks = tasks;
		CpuSeconds = cpuSeconds;
	}
}