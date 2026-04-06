using AspNetPractice.Data;  
using AspNetPractice.Models;    
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Players.db"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/player", async (Player player, AppDbContext db) =>
{
    await db.Players.AddAsync(player);
    await db.SaveChangesAsync();            
    return Results.Created($"/player/{player.Id}", player);
});

app.MapGet("/players", async (AppDbContext db) =>
{
    return await db.Players.ToListAsync();
});

app.MapGet("/player/{id}", async (int id, AppDbContext db) =>
{
    return await db.Players.FindAsync(id);
});

app.MapPut("/player/{id}", async (int id, Player updatedPlayer, AppDbContext db) =>
{
    Player player = await db.Players.FindAsync(id);

    if(player != null)
    {
        player.Name = updatedPlayer.Name;
        player.Level = updatedPlayer.Level;
        
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapDelete("/player/{id}", async (int id, AppDbContext db) =>
{
    Player player = await db.Players.FindAsync(id);

    if(player != null)
    {
        db.Players.Remove(player);
        await db.SaveChangesAsync();

        return Results.Ok();
    }
    else
    {
        return Results.NotFound();
    }
});


app.Run();