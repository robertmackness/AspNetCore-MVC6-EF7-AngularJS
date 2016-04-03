using AutoMapper;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;

// This Controller doesn't return IActionView implementations, it returns a JsonResult as it's interacting
// with the database. Key Features include:
//  1.  Default Route Metadata
//      Setting default route matches using [Route()] metadata for the entire controller, so that Actions
//      can use this as default. We're building a RESTful API so the route is the same but the Http methods change.
//  2.  AutoMapper
//      We're using the AutoMapper NuGet Package to map from a ViewModel to a real data Model, taking the values across

namespace TheWorld.Controllers.Api
{
    // Stops belong to trips, so we need
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        private CoordService _coordService;
        private ILogger _logger;
        private IWorldRepository _repository;

        public StopController(IWorldRepository repository, ILogger<StopController> logger, CoordService coordService)
        {
            _logger = logger;
            _repository = repository;
            _coordService = coordService;
        }

        [HttpGet("")]
        public JsonResult GET(string tripName)
        {
            try
            {
                var results = _repository.GetTripByName(tripName);
                if(results == null)
                {
                    return Json(new { Message = "None found"});
                }
                return Json(AutoMapper.Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(s => s.Order)));
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to get trip by name {tripName}", ex);
                return Json(new { Message = "Failed to get trip by name", Error = ex.Message });
            }
        }

        [HttpPost("")]
        public async Task<JsonResult> Post(string tripName, [FromBody]StopViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 1. Map to the data model
                    var newStop = Mapper.Map<Stop>(viewModel);
                    // 2. Lookup GeoCo-Ordinates
                    var coordResult = await _coordService.Lookup(newStop.Name);
                    if (!coordResult.Success)
                    {
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        return Json(coordResult.Message);
                    }else
                    {
                        newStop.Longitude = coordResult.Longitude;
                        newStop.Latitude = coordResult.Latitude;
                    }
                    // 3. Save to the database
                    _repository.AddStop(newStop, tripName);
                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new stop", ex);
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json("Failed to save new stop");
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("Validation failed on new Stop");
        }
    }
}
