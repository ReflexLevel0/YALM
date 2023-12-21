using YALM.Common.Models;

namespace YALM.API.Models.Db;

public class MemoryDbLog : DbLogBase
{
	public int ServerId { get; }
	public int MbUsed { get; }
	public int MbTotal { get; }
	
	[GraphQLIgnore]
	public double Usage => (double)MbUsed / MbTotal;

	public MemoryDbLog(int serverId, DateTime date, int interval, int mbUsed, int mbTotal) : base(date, interval)
	{
		MbUsed = mbUsed;
		MbTotal = mbTotal;
	}
}