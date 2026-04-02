using AspNetPractice.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetPractice.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}