using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Oishipan.Models
{
    public class OishipanContextFactory : IDesignTimeDbContextFactory<OishipanContext>
    {
        public OishipanContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OishipanContext>();
            
            var connectionString = "Server=DESKTOP-H3S2KUE\\HARYCUTE;Database=Oishipan;Trusted_Connection=true;TrustServerCertificate=true;";
            
            optionsBuilder.UseSqlServer(connectionString);

            return new OishipanContext(optionsBuilder.Options);
        }
    }
}
