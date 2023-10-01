using API;

var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddSingleton(new Db(File.ReadAllText("dbConnectionString.txt")))
	.AddGraphQLServer()
	.AddQueryType<Query>();

var app = builder.Build();
app.MapGraphQL();
await app.RunAsync();