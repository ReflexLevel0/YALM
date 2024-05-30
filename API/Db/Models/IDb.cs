using DataModel;
using LinqToDB;

namespace YALM.API.Db.Models;

public interface IDb : IDataContext
{
	public ITable<CpuDbRecord> Cpus { get; }
	public ITable<CpuLogDbRecord> CpuLogs { get; }
	public ITable<DiskDbRecord> Disks { get; }
	public ITable<MemoryLogDbRecord> MemoryLogs { get; }
	public ITable<PartitionDbRecord> Partitions { get; }
	public ITable<PartitionLogDbRecord> PartitionLogs { get; }
	public ITable<ProgramLogDbRecord> ProgramLogs { get; }
	public ITable<ServiceDbRecord> Services { get; }
	public ITable<ServiceLogDbRecord> ServiceLogs { get; }
	public ITable<ServicenameDbRecord> ServiceNames { get; }
	public ITable<ServicestatusDbRecord> ServiceStatuses { get; }
	ITable<AlertDbRecord> Alerts { get; }
}