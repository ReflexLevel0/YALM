using API.Models;

namespace API;

public class Query
{
	public Cpu Cpu()
	{
		return new Cpu(0, DateTime.Today, 5, 0.1, 13);
	}

	public Ram Ram()
	{
		return new Ram(0, DateTime.Today, 5, 1512, 8192);
	}
}