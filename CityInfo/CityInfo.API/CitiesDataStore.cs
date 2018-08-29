using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            Cities = new List<CityDto>
            {
                new CityDto
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "NYC Desc",
                    PointsOfInterest = new List<PointsOfInterestDto>()
                    {
                        new PointsOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "Central Park Description"
                        },
                        new PointsOfInterestDto()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "Description Empire State Building"
                        }
                    }
                },
                new CityDto
                {
                    Id = 2,
                    Name = "Athens",
                    Description = "Athens Desc",
                    PointsOfInterest = new List<PointsOfInterestDto>()
                    {
                        new PointsOfInterestDto()
                        {
                            Id = 4,
                            Name = "Acropolis",
                            Description = "Description Acropolis"
                        }
                    }
                    
                },
                new CityDto
                {
                    Id = 3,
                    Name = "Zurich",
                    Description = "Zurich Desc"
                }
            };
        }
    }
}
