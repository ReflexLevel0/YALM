using API;

var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddSingleton(new Db(File.ReadAllText("dbConnectionString.txt")))
	.AddGraphQLServer()
	.AddQueryType<Query>();
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