using YALM.API;
using YALM.API.Db;
using YALM.API.GraphQL.Alerts;
using YALM.API.GraphQL.Mutations;
using YALM.API.GraphQL.Query;

var dbProvider = new MonitoringDbProvider();
var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddScoped<IDbProvider>(_ => dbProvider)
	.AddScoped<IMutationHelper>(_ => new MutationHelper(dbProvider))
	.AddSingleton<IAlertHelper>(_ => new AlertHelper(dbProvider))
	.AddGraphQLServer()
	.AddQueryType<Query>()
	.AddMutationType(m => m.Name("Mutation"))
	.AddType<CpuMutation>()
	.AddType<CpuLogMutation>()
	.AddType<MemoryLogMutation>()
	.AddType<PartitionMutation>()
	.AddType<PartitionLogMutation>()
	.AddType<ProgramLogMutation>()
	.AddType<DiskMutation>()
	.AddType<ServerMutation>();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", b =>
	{
		b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
	});
});

var app = builder.Build();
app.UseCors("AllowAll");
app.MapGraphQL();
await app.RunAsync();