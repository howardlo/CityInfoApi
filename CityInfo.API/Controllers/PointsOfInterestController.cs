using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,
        ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/PointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound($"City with id: {cityId} not found");
            }

            var pointsOfInterest = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
            var pointsOfInterestResult = new List<PointOfInterestDto>();

            foreach (var poi in pointsOfInterest)
            {
                pointsOfInterestResult.Add(new PointOfInterestDto
                {
                    Id = poi.Id,
                    Name = poi.Name,
                    Description = poi.Description
                });
            }

            return Ok(pointsOfInterestResult);

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    // _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
            //    _logger.LogCritical($"City with id {cityId} wasn't found when accessing points of interest.");
            //    return NotFound();
            //}
            //return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/PointsOfInterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound(new { message = $"City with id: {cityId} is not found" });
            }

            var poi = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (poi == null)
            {
                return NotFound($"PointOfInterest with id: {id} is not found");
            }

            var pointOfInterestResult = new PointOfInterestDto
            {
                Id = poi.Id,
                Name = poi.Name,
                Description = poi.Description
            };

            return Ok(pointOfInterestResult);

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{


            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
            //    return NotFound();
            //}

            //var poi = city.PointsOfInterest.SingleOrDefault(p => p.Id == id);
            //if (poi == null)
            //{
            //    return NotFound();
            //}

            //return Ok(poi);
        }

        [Route("{cityId}/PointsOfInterest")]
        [HttpPost()]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            var b = Request.Body;
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                id = finalPointOfInterest.Id
            }, finalPointOfInterest);
        }

        [Route("{cityId}/PointsOfInterest/{id}")]
        [HttpPut()]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            var b = Request.Body;
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.SingleOrDefault(e => e.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [Route("{cityId}/PointsOfInterest/{id}")]
        [HttpPatch()]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {

            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.SingleOrDefault(e => e.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [Route("{cityId}/PointsOfInterest/{id}")]
        [HttpDelete()]
        public IActionResult DeletePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.SingleOrDefault(e => e.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            _mailService.Send($"Delete {pointOfInterestFromStore.Id}", $"Deleting {pointOfInterestFromStore.Name} (id: {pointOfInterestFromStore.Id})");

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            return NoContent();
        }
    }
}
