using LinqToDB.Mapping;

namespace YALM.API.Db.Models;

[Table("server")]
public class ServerDbRecord
{
	[Column("serverid"     , IsPrimaryKey = true , PrimaryKeyOrder = 0                        )] public int      Serverid      { get; set; } // integer
}