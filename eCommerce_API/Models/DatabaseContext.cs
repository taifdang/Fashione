using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Data;

namespace eCommerce_API.Models
{
    public class DatabaseContext : DbContext
    {
       public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options)
       {

       }

        //DbSet
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryType> CategoriesType { get; set; }
                 
        //Seeding
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>()
                .HasMany(p => p.products)
                .WithOne(c => c.category)
                .HasForeignKey(e => e.categoryId)
                .IsRequired(false);         
            modelBuilder.Entity<CategoryType>()
                .HasMany(p => p.category)
                .WithOne(p=>p.categoryType)
                .HasForeignKey(e=>e.categoryTypeId)
                .IsRequired(false);
        }
    }
}
