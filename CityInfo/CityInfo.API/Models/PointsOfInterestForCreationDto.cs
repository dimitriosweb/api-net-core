﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class PointsOfInterestForCreationDto
    {
        [Required(ErrorMessage = "Should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
