using EventSourcing.WithoutAggregates.API.Data.EventStore;
using EventSourcing.WithoutAggregates.API.Data.Sql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddBankAccountsSql()
    .AddEventStoreDB();

var app = builder.Build();

app.MapControllers();

app.Run();
