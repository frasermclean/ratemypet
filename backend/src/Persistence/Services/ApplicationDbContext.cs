using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;

namespace RateMyPet.Persistence.Services;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostReaction> PostReactions => Set<PostReaction>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<Species> Species => Set<Species>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // rename Identity tables
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
