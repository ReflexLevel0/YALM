namespace Common.Models;

public interface IDbLogBase
{ 
	int ServerId { get; }
	DateTime Date { get; }
	int Interval { get; }
}