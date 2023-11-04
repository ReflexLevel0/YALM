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
	CpuDbLog ParseCpuRecord(NpgsqlDataReader reader);

	/// <summary>
	/// Parses a database record and turns it into <see cref="MemoryDbLog"/>
	/// </summary>
	/// <param name="reader"></param>
	/// <returns></returns>
	MemoryDbLog ParseMemoryRecord(NpgsqlDataReader reader);

	/// <summary>
	/// Parses a database record and turns it into <see cref="StorageDbLog"/>
	/// </summary>
	/// <param name="reader"></param>
	/// <returns></returns>
	StorageDbLog ParseStorageRecord(NpgsqlDataReader reader);
}