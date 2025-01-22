using ProdcutCatalog.Entities;
using System;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProdcutCatalog.DbContexts
{
    public class ProductCatalogDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : base(options)
        {
        }
    }

}
