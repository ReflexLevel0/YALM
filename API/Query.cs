using API.Models;

namespace API;

public class Query
{
	public Cpu Cpu()
	{
		return new Cpu
		{
			Processes =
			{
				new ProcessCpuInfo("java", 50), new ProcessCpuInfo("sshd", 10.5)
			}, Usage = 60.5, NumberOfTasks = 2
		};
	}
}