using DataModel;
using LinqToDB;
using YALM.API;
using YALM.API.Models.Db;
using YALM.API.Mutations;

string connectionString = File.ReadAllText("dbConnectionString.txt");
var dataOptions = new DataOptions<MonitoringDb>(new DataOptions().UsePostgreSQL(connectionString));
var db = new MonitoringDb(dataOptions);
var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddSingleton<IDb>(db)
	.AddTransient<IMutationHelper>(_ => new MutationHelper(db))
	.AddGraphQLServer()
	.AddQueryType<Query>()
	.AddMutationType(m => m.Name("Mutation"))
	.AddType<CpuMutation>()
	.AddType<CpuLogMutation>()
	.AddType<MemoryLogMutation>()
	.AddType<PartitionMutation>()
	.AddType<PartitionLogMutation>()
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