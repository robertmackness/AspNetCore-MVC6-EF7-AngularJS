using System;
using System.ComponentModel.DataAnnotations;

namespace TheWorld.Controllers.Api
{
    public class StopViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        [Required]
        public DateTime Arrival { get; set; }
    }
}