using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

// This Controller doesn't return IActionView implementations, it returns JsonResult as it's interacting
// with the database. Key Features include:
//  1.  Default Route Metadata
//      Setting default route matches using [Route()] metadata for the entire controller, so that Actions
//      can use this as default. We're building a RESTful API so the route is the same but the Http methods change.
//  2.  AutoMapper
//      We're using the AutoMapper NuGet Package to map from a ViewModel to a real data Model, taking the values across

namespace TheWorld.Controllers.Api
{
    //  1.  Default Route Metadata
    //      Setting default route matches using [Route()] metadata for the entire controller, so that Actions
    //      can use this as default. We're building a RESTful API so the route is the same but the Http methods change.
    [Authorize]
    [Route("api/trips")]
    public class TripController : Controller
    {
        private ILogger<TripController> _logger;
        private IWorldRepository _repository;

        public TripController(IWorldRepository repository, ILogger<TripController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            // Don't return the full Trip model data, use Mapper to map it across to the TripViewModel
            // Note that _repository.GetAllTripsWithStops() returns an IEnumerable implementation, so
            // we have to give Mapper that information
            var trips = _repository.GetUserTripsWithStops(User.Identity.Name);
            var results = Mapper.Map <IEnumerable<TripViewModel>>(trips);
            return Json(results);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody]TripViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //  2.  AutoMapper
                    //      We're using the AutoMapper NuGet Package to map from a ViewModel to a real data Model, taking the values across
                    var newTrip = Mapper.Map<Trip>(viewModel);
                    // Set the new trip's username to the currently logged in user.  we can make this assumption as the entire TripController only gets
                    // called if the Identity service recoginises that a user has logged in and is therefore [Authorize]'d
                    newTrip.UserName = User.Identity.Name;
                    // Here we queue up the new Trip for the repository to handle while sending a Information Log to the sytem
                    _logger.LogInformation("Attempting to save new Trip to the database", newTrip);
                    _repository.AddTrip(newTrip);

                    // _repository.SaveAll() returns a boolean if all newly added items have been saved to the DB successfully
                    // this pattern will allow us to add more models to the repository to handle before calling a save, meaning less
                    // overhead on the DB
                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<TripViewModel>(newTrip));
                    }
                    
                }
                return Json(new { Message = "Model invalid" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save trip", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
        }
    }
}
