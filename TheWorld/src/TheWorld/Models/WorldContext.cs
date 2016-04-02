using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace TheWorld.Models
{

    // This class derives from DbContext and is used to create the tables/properties encapsulated within one topic, 
    // in this case the World with it's Trips and Stops.
    // During Startup.ConfigureServices() we add the EntityFramework, add a Sql server, then add this context to the DB.

    public class WorldContext : DbContext
    {
        // Add the DbSets to this context 
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }

        public WorldContext()
        {
            Database.EnsureCreated(); // Create DB if not already
        }

        // We're overriding the OnConfiguring method here called via Startup.ConfigureServices() to tell this context 
        // to use the options of: UseSqlServer with an argument of the DB string from Startup.Configuration which is an IConfigurationRoot implementation
        // pointing to ~/config.json
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = Startup.Configuration["Data:WorldContextConnection"];
            optionsBuilder.UseSqlServer(connString);
        }
    }
}
