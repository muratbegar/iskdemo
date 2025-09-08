using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Outbox
{
    public class OutboxDbContextFactory : IDesignTimeDbContextFactory<OutboxDbContext>
    {
        public OutboxDbContext CreateDbContext(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<OutboxDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ElearningIskoop;Username=postgres;Password=1234;"); // connection string

            return new OutboxDbContext(optionsBuilder.Options);
        }
    }
}
