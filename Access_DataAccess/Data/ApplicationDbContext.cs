using Access_Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Access_DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<AttributeValue> AttributeValue { get; set; }
        public DbSet<AttributeType> AttributeType { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<InquiryDetail> InquiryDetail { get; set; }
        public DbSet<InquiryHeader> InquiryHeader { get; set; }
        public DbSet<InquiryToOrder> InquiryToOrder { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductAttribute> ProductAttribute { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
    }
}
