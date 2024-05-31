using HotChocolate.Language;
using LinqToDB;
using YALM.API.Db;
using YALM.API.Db.Models;
using YALM.Common.Models.Graphql;
using YALM.Common.Models.Graphql.InputModels;
using YALM.Common.Models.Graphql.OutputModels;

namespace YALM.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class ServerMutation(IMutationHelper mutationHelper, IDbProvider dbProvider)
{
	private readonly Func<ServerInput, IQueryable<ServerDbRecord>> _getServerQuery = server =>
		from s in dbProvider.GetDb().Servers
		where s.ServerId == server.ServerId
		select s;

	private readonly Func<ServerDbRecord, ServerInput> _getServerId = server => new ServerInput(server.ServerId);
	
	public async Task<Payload<ServerOutputBase>> AddOrReplaceServer(ServerInput server)
	{
		var model = InputToDbModel(server);
		
		//TODO: Not inserting the server if it already exists (THIS IS BAD CODE FOR SOME REASON JUST DOING AddOrReplaceModelAsync CRASHES WITH
		//TODO: "There are no fields to update in the type 'server'." so this is just a workaround, it should be fixed and this method should behave like in other mutations 
		await using var db = dbProvider.GetDb();
		var serv = await (from s in db.Servers where s.ServerId == server.ServerId select s).FirstOrDefaultAsync();
		if (serv != null) return new Payload<ServerOutputBase>{Data = new ServerOutputBase(serv.ServerId), Error = null};
		
		return await mutationHelper.AddModelAsync<ServerInput, ServerDbRecord, ServerOutputBase>(model, _getServerId(model), _getServerQuery);
	}

	private static ServerDbRecord InputToDbModel(ServerInput server)
	{
		return new ServerDbRecord { ServerId = server.ServerId };
	}
}