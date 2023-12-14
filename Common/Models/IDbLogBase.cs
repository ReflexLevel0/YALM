namespace Common.Models;

public interface IDbLogBase
{ 
	DateTime Date { get; }
	int Interval { get; }
}