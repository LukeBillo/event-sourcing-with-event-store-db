using EventSourcing.Aggregates.API.Data.EventStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddEventStoreDB();

var app = builder.Build();

app.MapControllers();

app.Run();
