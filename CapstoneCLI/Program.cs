using Capstone.DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Capstone
{
    static class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            CampgroundSqlDAL db = new CampgroundSqlDAL(connectionString);

            CapstoneCLI capstoneCli = new CapstoneCLI(db);

            capstoneCli.Start();
        }
    }
}
