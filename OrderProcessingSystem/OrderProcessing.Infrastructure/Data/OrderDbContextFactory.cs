using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Infrastructure.Data
{
    public class OrderDbContextFactory
    : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=OrderProcessingDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}
