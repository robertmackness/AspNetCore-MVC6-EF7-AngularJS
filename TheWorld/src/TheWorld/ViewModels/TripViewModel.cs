using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TheWorld.Controllers.Api;

namespace TheWorld.ViewModels
{
    public class TripViewModel
    {

        public string Id { get; set; }

        [Required]
		[StringLength(255, MinimumLength =5)]
        public string Name { get; set; }

        public DateTime Created { get; set; }

        public IEnumerable<StopViewModel> Stops { get; set; }

    }
}
