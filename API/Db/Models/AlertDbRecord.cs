using LinqToDB.Mapping;

namespace YALM.API.Db.Models;

[Table("alert")]
public class AlertDbRecord
{
	[Column("serverid"     , IsPrimaryKey = true , PrimaryKeyOrder = 0                        )] public int      Serverid      { get; set; } // integer
	[Column("date"         , IsPrimaryKey = true , PrimaryKeyOrder = 2                        )] public DateTime Date          { get; set; } // timestamp (6) without time zone
	[Column("severity"                                                                        )] public int      Severity      { get; set; } // integer
	[Column("text"                                                                            )] public required string   Text          { get; set; } // varchar(256)
}