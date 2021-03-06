using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gameblasts.Models;
using Gameblasts.Models.CategoryModels;

namespace Gameblasts.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Message> Messages { get; set; }
        
        public DbSet<CategoryModel> Categories {get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>()
                    .HasMany<Post>(b => b.Posts);

            builder.Entity<ApplicationUser>()
                    .HasMany<Message>(b => b.Messages);
            
            builder.Entity<CategoryModel>()
                .HasMany<CategoryModel>(b => b.children);

            builder.Entity<CategoryModel>().HasOne<CategoryModel>(b => b.parent);

           
        }
    }
}
