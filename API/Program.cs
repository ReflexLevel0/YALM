using YALM.API;
using YALM.API.Alerts;
using YALM.API.Db;
using YALM.API.Mutations;

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
	.AddType<DiskMutation>();
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