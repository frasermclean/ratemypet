using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RateMyPet.Database;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        const string connectionString =
            "Server=127.0.0.1;Database=RateMyPet;User ID=sa;Password=sup3rSECRET!;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString).Options;

        return new ApplicationDbContext(options);
    }
}
