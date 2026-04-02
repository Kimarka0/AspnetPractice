using AspNetPractice.Models;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

List<Player> players = new();

app.MapGet("/player/{id}", (int id) => new Player{Name = "Chupep", Level = 1488});
app.MapGet("/players", () => players);
app.MapGet("/player/search", (string name, int level) => new Player{Name = name, Level = level});

app.MapPost("/player", (Player player) => 
{   
    players.Add(player);
    return Results.Created(player.Name, player);
});

app.Run();

