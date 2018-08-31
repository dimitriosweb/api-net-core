using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController :  Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,ICityInfoRepository cityRepositoryInfo)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityRepositoryInfo;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //throw new Exception("Exception sample");
                var pointsOfInterestForACity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                if (!_cityInfoRepository.CityExists(cityId))
                //if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var pointsOfInterestForCityResults =
                    Mapper.Map<IEnumerable<PointsOfInterestDto>>(pointsOfInterestForCity);

                return Ok(pointsOfInterestForCityResults);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", e);
                return StatusCode(500,"A problem happened while handling your request.");
            }
            
        }

        [HttpGet("{cityId}/pointofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterest == null)
                return NotFound();

            var pointOfInterestResult = Mapper.Map<PointsOfInterestDto>(pointOfInterest);
            return Ok(pointOfInterestResult);

        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (pointOfInterest.Name == pointOfInterest.Description)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);
            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);
            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem happened while handling your request.");

            var createdPointOfInterestToReturn = Mapper.Map<Models.PointsOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new {cityId = cityId, id = createdPointOfInterestToReturn.Id}, createdPointOfInterestToReturn);
        }

        [HttpPut("{cityId}/pointofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (pointOfInterest.Name == pointOfInterest.Description)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            Mapper.Map(pointOfInterest, pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem happened while handling your request.");
            return NoContent();
        }

        [HttpPatch("{cityId}/pointofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
                return NotFound();

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
                return NotFound();

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Name;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
                return NotFound();
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
                return NotFound();
            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest deleted.", $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

            return NoContent();
        }
    }
}
