using aspnetPractice.Models;
using aspnetPractice.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=clients.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/client", async (Client client, AppDbContext db) =>
{
    await db.Clients.AddAsync(client);
    await db.SaveChangesAsync();
    
    return Results.Created($"/client/{client.Id}", client);
});

app.MapGet("/client/{id}", async (int id, AppDbContext db) =>
{
    Client client = await db.Clients.FindAsync(id);

    if(client == null) return Results.NotFound();
    
    return Results.Ok(client);
});

app.MapGet("/clients", async (AppDbContext db) =>
{

    return await db.Clients.ToListAsync();
});

app.MapPut("/client/{id}", async (int id, Client updatedClient, AppDbContext db) =>
{
    Client client = await db.Clients.FindAsync(id);

    if(client == null) return Results.NotFound();

    client.Name = updatedClient.Name;
    client.Surname = updatedClient.Surname;
    client.Age = updatedClient.Age;
    client.Balance = updatedClient.Balance;

    await db.SaveChangesAsync();

    return Results.Ok();
    
});

app.MapDelete("/client/{id}", async (int id, AppDbContext db) =>
{
    Client client = await db.Clients.FindAsync(id);

    if(client == null) return Results.NotFound();

    db.Clients.Remove(client);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

