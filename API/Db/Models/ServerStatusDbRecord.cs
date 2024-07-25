using LinqToDB.Mapping;

namespace YALM.API.Db.Models;

[Table("serverstatus")]
public class ServerStatusDbRecord
{
	[Column("serverid"     , IsPrimaryKey = true , PrimaryKeyOrder = 0)] public int      Serverid      { get; set; } // integer
	[Column("status")] public string Status { get; set; }
}