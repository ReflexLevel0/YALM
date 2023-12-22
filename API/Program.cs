using DataModel;
using LinqToDB;
using YALM.API;
using YALM.API.Models.Db;

string connectionString = File.ReadAllText("dbConnectionString.txt");
var dataOptions = new DataOptions<MonitoringDb>(new DataOptions().UsePostgreSQL(connectionString));
var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddSingleton<IDb>(new MonitoringDb(dataOptions))
	.AddGraphQLServer()
	.AddQueryType<Query>()
	.AddMutationType<Mutation>();
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