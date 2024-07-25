using LinqToDB.Mapping;

namespace YALM.API.Db.Models;

[Table("serverlog")]
public class ServerLogDbRecord
{
	[Column("serverid"     , IsPrimaryKey = true , PrimaryKeyOrder = 0)] public int      Serverid      { get; set; } // integer
	[Column("date"         , IsPrimaryKey = true , PrimaryKeyOrder = 1)] public DateTimeOffset Date          { get; set; } // timestamp (6) without time zone
	[Column("interval"                                                )] public int      Interval      { get; set; } // integer
}