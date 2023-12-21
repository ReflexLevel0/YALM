namespace YALM.Common.Models.Graphql;

public interface ILoggingBase<T> where T : LogBase
{
	List<T> Logs { get; }
}