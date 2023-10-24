namespace API.Models.Db;

public class RamLog : LogBase
{
	public int MbUsed { get; }
	public int MbTotal { get; }
	public double Usage => (double)MbUsed / MbTotal;

	public RamLog(int serverId, DateTime date, int interval, int mbUsed, int mbTotal) : base(serverId, date, interval)
	{
		MbUsed = mbUsed;
		MbTotal = mbTotal;
	}
}