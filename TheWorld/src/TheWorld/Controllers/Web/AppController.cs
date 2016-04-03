using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TheWorld.ViewModels;
using TheWorld.Services;
using TheWorld.Models;
using Microsoft.AspNet.Authorization;


// This is our main AppController, which the http pipeline defaults to if other controllers and actions are not specified.
// MVC 6 knows to look for this controller because of the directory structure, and naming of the class.
// It inherits from AspNet.Mvc.Controller to give it out-of-the-box functionality.
//
// Key parts:
// 1.  Constructor. 
//     Using dependency injection to grabs an IMailService interface, the concrete implementation it gets depends on the
//     Startup.cs if block concerning whether the app is running in DEBUG mode or not.
//     It also gets an IWorldRepository interface implementation from Startup.cs for interacting with the database.
//
// 2. IActionResults.
//    These are AspNet.Mvc interfaces that you can customise. These are called from the routes defined in Startup.Configure().
//    The views which they return are methods which return a rendered view to the client, and these map to ~/Views/*.cshtml with a
//    matching name.
//    It's possible to attach metadata to the Action, e.g. [HttpPost] which can be matched by AspNet.Mvc using reflection, for finer
//    control over matching routes.
//
// 3. ViewModels
//    These are stripped down versions you create of your data models that don't expose the full model to the client
//    E.g.: You would omit id and created_at fields because these could be generated server-side.
//    These are also useful for data validation using metadata and reflection e.g.: [Required] [StringLength(255, MinimumLength =3)]

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IWorldRepository _repository;

        // 1.  Constructor. 
        //     Using dependency injection to grabs an IMailService interface, the concrete implementation it gets depends on the
        //     Startup.cs if block concerning whether the app is running in DEBUG mode or not.
        //     It also gets an IWorldRepository interface implementation from Startup.cs for interacting with the database
        public AppController(IMailService service, IWorldRepository repository)
        {
            _mailService = service;
            _repository = repository;
        }

        // 2. IActionResults.
        //    These are AspNet.Mvc interfaces that you can customise. These are called from the routes defined in Startup.Configure().
        //    The views which they return are methods which return a rendered view to the client, and these map to ~/Views/*.cshtml with a
        //    matching name.
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        // 3. ViewModels
        //    These are stripped down versions you create of your data models that don't expose the full model to the client
        //    E.g.: You would omit id and created_at fields because these could be generated server-side.
        //    These are also useful for data validation using metadata and reflection e.g.: [Required] [StringLength(255, MinimumLength =3)]
        [HttpPost] // Attach metadata to this IActionResult signifying this Action should match /App/Contact with method: Post
        public IActionResult Contact(ContactViewModel model)
        {
            // this is an AspNet.Mvc value, it checks if there are any errors on submitted values in ModelStateDictionary
            // It uses the view model provided on the contact page, which references ContactViewModel, which has fields
            // assigned various metadata
            if (ModelState.IsValid) 
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"]; // Startup.Configuration points to ~/config.json via IConfigurationRoot implementation
                _mailService.SendMail(
                    email,
                    email,
                    $"Sent from contact Page by {model.Name} ({model.Email})",
                    model.Message
                );
                ModelState.Clear(); // Clear out the ModelState
                ViewBag.Message = "Mail Sent. Thanks.";
            }
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            var trips = _repository.GetAllTrips();
            return View(trips);
        }
    }
}
