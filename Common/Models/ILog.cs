namespace YALM.Common.Models;

public interface ILog
{ 
	DateTimeOffset Date { get; }
	int Interval { get; }
}