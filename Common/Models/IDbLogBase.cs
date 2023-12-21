namespace YALM.Common.Models;

public interface IDbLogBase
{ 
	DateTime Date { get; }
	int Interval { get; }
}