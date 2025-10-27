using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Products;

namespace ProductManagement.Persistence;

public class ProductManagementContext : DbContext
{
    public DbSet<Product> Products { get; set; }
}