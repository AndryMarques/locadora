using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace locadora.Data
{
    public class LocadoraDbContextFactory : IDesignTimeDbContextFactory<LocadoraDbContext>
    {
        public LocadoraDbContext CreateDbContext(string[] args)
        {
            // Constrói a configuração a partir do arquivo appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<LocadoraDbContext>();
            builder.UseSqlServer(connectionString);

            return new LocadoraDbContext(builder.Options);
        }
    }
}