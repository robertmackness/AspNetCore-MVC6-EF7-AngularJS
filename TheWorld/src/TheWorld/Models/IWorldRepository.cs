using System.Collections.Generic;

namespace TheWorld.Models
{
    // This is solely created to provide the ability to easily fake an implementation of the real repository for testing purposes
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetAllTripsWithStops();
        void AddTrip(Trip newTrip);
        bool SaveAll();
        Trip GetTripByName(string tripName, string userName);
        void AddStop(Stop newStop, string tripName, string userName);
        IEnumerable<Trip> GetUserTripsWithStops(string name);
    }
}