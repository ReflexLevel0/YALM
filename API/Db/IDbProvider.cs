using YALM.API.Db.Models;

namespace YALM.API.Db;

public interface IDbProvider
{
	IDb GetDb();
}