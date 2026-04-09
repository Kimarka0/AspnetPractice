using aspnetPractice.Models;
using aspnetPractice.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;



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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введи JWT токен"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
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


app.MapPost("/login", async (LoginModel loginModel, AppDbContext db) =>
{
    Client? client = await db.Clients.FirstOrDefaultAsync(client => client.Name == loginModel.Username);

    if(client == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, client.PasswordHash))
    {
        return Results.Unauthorized();
    }

    List<Claim> claims = new()
    {
        new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
        new Claim(ClaimTypes.Name, client.Name),
        new Claim(ClaimTypes.Role, "Client")
    };

    SigningCredentials signingCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
    SecurityAlgorithms.HmacSha256
    );

    JwtSecurityToken token = new(
        issuer: jwtSettings["Issuer"],
        audience: jwtSettings["Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: signingCredentials
        );
    
    string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new {Token = tokenString});
});


app.MapDelete("/client/{id}", async (int id, AppDbContext db) =>
{
    Client client = await db.Clients.FindAsync(id);

    if(client == null) return Results.NotFound();

    db.Clients.Remove(client);

    await db.SaveChangesAsync();

    return Results.NoContent();
}
).RequireAuthorization();

app.MapGet("/secret", () => "top secret!")
    .RequireAuthorization();


app.Run();

