using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// The Repository Pattern
// This is used to control and manage all of your database interactions in one location, to avoid dispersed calls to the DB
// and to adhere to the Do Not Repeat Yourself principle.

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository>  logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddTrip(Trip newTrip)
        {
            _context.Add(newTrip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            try
            {
                return _context.Trips.OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get trips from database", ex);
                return null;
            }
            
        }

        public IEnumerable<Trip> GetAllTripsWithStops()
        {
            try
            {
                return _context.Trips
                    .Include(t => t.Stops)
                    .OrderBy(t => t.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get trips with stops from database", ex);
                return null;
            }
        }

        public Trip GetTripByName(string tripName)
        {
           return  _context.Trips.Include(t => t.Stops)
                                        .Where(t => t.Name == tripName)
                                        .FirstOrDefault();
        }

        public bool SaveAll()
        {
            // _context.SaveChanges returns how many changes were made, so if it's more than 0 we can evaluate a bool expression for true
            return _context.SaveChanges() > 0;
        }
    }
}
