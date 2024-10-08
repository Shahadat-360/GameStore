using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreDbContext>(connString);
var app = builder.Build();
app.MapGetEndpoints();
app.MigrateDb();
app.Run();
