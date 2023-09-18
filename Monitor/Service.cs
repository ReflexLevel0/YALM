namespace Monitor;

public class Service
{
	public string Name { get; }
	public string Active { get; set; }
	public int Tasks { get; set; }
	public string Memory { get; set; }
	public string Cpu { get; set; }

	public Service(string name)
	{
		Name = name;
	}

	public override string ToString()
	{
		return $"Service: {Name}\nActive: {Active}\nTasks: {Tasks}\nMemory: {Memory}\nCpu: {Cpu}\n";
	}
}