using Microsoft.EntityFrameworkCore;
using CoordinatorService.Domain;

namespace CoordinatorService.DB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Agent> Agents { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
    }
}