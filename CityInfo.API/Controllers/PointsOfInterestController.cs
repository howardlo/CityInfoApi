using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/")]
    public class PointsOfInterestController : Controller
    {
        [HttpGet("{cityId}/PointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/PointsOfInterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterest.SingleOrDefault(p => p.Id == id);
            if (poi == null)
            {
                return NotFound();
            }

            return Ok(poi);
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

            if( !ModelState.IsValid )
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

            if( pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if( !ModelState.IsValid )
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
    }
}
