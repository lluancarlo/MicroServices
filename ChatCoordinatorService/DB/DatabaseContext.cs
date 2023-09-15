using Microsoft.EntityFrameworkCore;
using ChatCoordinatorService.Domain;

namespace ChatCoordinatorService.DB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Agent> Agents { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
    }
}
