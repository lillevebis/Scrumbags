using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gameblasts.Models;

namespace Gameblasts.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<CategoryModels.TopCategoryModel> topCategories {get; set; }
        public DbSet<CategoryModels.SubCategoryModel> subCategories { get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>()
                    .HasMany<Post>(b => b.Posts);
            
            builder.Entity<CategoryModels.TopCategoryModel>()
                .HasMany<CategoryModels.SubCategoryModel>(b => b.children);

           
        }
    }
}
