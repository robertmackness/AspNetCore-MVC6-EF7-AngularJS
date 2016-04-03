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
        Trip GetTripByName(string tripName);
        void AddStop(Stop newStop, string tripName);
    }
}