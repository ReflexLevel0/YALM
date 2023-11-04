using Common.Models.Graphql.InputModels;

namespace Monitor.Models;

public class GraphqlVariables
{
	public CpuInput cpu;
	public MemoryInput memory;
	public StorageInput storage;
}