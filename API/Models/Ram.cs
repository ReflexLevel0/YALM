namespace API.Models;

public class Ram : Log
{
	public int MbUsed { get; }
	public int MbTotal { get; }
	public double Usage => (double)MbUsed / MbTotal;

	public Ram(int serverId, DateTime date, int interval, int mbUsed, int mbTotal) : base(serverId, date, interval)
	{
		MbUsed = mbUsed;
		MbTotal = mbTotal;
	}
}