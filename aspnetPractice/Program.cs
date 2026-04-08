using aspnetPractice.Models;
using aspnetPractice.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

IConfiguration jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=clients.db"));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/client", async (Client client, AppDbContext db, ClientValidator validator) =>
{
    ValidationResult validationResult = await validator.ValidateAsync(client);

    if(!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

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

app.MapPut("/client/{id}", async (int id, Client updatedClient, AppDbContext db, ClientValidator validator) =>
{
    Client client = await db.Clients.FindAsync(id);

    if(client == null) return Results.NotFound();

    ValidationResult validationResult = await validator.ValidateAsync(updatedClient);

    if(!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    client.Name = updatedClient.Name;
    client.Surname = updatedClient.Surname;
    client.Age = updatedClient.Age;
    client.Balance = updatedClient.Balance;

    await db.SaveChangesAsync();

    return Results.Ok(client);
    
}).RequireAuthorization();

app.MapDelete("/client/{id}", async (int id, AppDbContext db) =>
{
    Client client = await db.Clients.FindAsync(id);

    if(client == null) return Results.NotFound();

    db.Clients.Remove(client);

    await db.SaveChangesAsync();

    return Results.NoContent();
}
).RequireAuthorization();

app.MapGet("/secret", () => "top secret!!!")
    .RequireAuthorization();

app.Run();

