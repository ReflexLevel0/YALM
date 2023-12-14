using API.Models.Db;
using Npgsql;

namespace API;

public interface IDb
{
	Task<NpgsqlConnection> OpenConnectionAsync();

	/// <summary>
	/// Executes SQL query and returns all data asynchronously
	/// </summary>
	/// <param name="command"></param>
	/// <returns></returns>
	IAsyncEnumerable<NpgsqlDataReader> ExecuteReaderAsync(string command);

	/// <summary>
	/// Executes SQL query and returns nothing
	/// </summary>
	/// <param name="command"></param>
	Task ExecuteNonQueryAsync(string command);

	/// <summary>
	/// Executes SQL query and returns only the first column of the first row (if it exists) 
	/// </summary>
	/// <param name="command"></param>
	/// <returns></returns>
	Task<object?> ExecuteScalarAsync(string command);

	/// <summary>
	/// Parses a database record and turns it into <see cref="CpuDbLog"/>
	/// </summary>
	/// <param name="reader"></param>
	/// <returns></returns>
	CpuDbLog ParseCpuLogRecord(NpgsqlDataReader reader);

	/// <summary>
	/// Parses a database record and turns it into <see cref="MemoryDbLog"/>
	/// </summary>
	/// <param name="reader"></param>
	/// <returns></returns>
	MemoryDbLog ParseMemoryLogRecord(NpgsqlDataReader reader);

	/// <summary>
	/// Parses a database record and turns it into <see cref="PartitionDbLog"/>
	/// </summary>
	/// <param name="reader"></param>
	/// <returns></returns>
	PartitionDbLog ParsePartitionLogRecord(NpgsqlDataReader reader);

	/// <summary>
	/// Parses a database record and turns it into <see cref="PartitionDb"/>
	/// </summary>
	/// <param name="reader"></param>
	/// <returns></returns>
	PartitionDb ParsePartitionRecord(NpgsqlDataReader reader);

	/// <summary>
	/// Parsed a database record and turns it into <see cref="DiskDb"/>
	/// </summary>
	/// <param name="reader"></param>
	/// <returns></returns>
	DiskDb ParseDiskRecord(NpgsqlDataReader reader);
}