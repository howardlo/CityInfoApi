using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [Route("api/cities")]
        [HttpGet()]
        public IActionResult GetCities()
        {
            // Original with CitiesDataStore
            // return new JsonResult( CitiesDataStore.Current.Cities );

            // Database and manual mapping
            //var cityEntities = _cityInfoRepository.GetCities();
            //var results = new List<CityWithoutPointsOfInterestDto>();
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}
            //return Ok(results);

            var cityEntities = _cityInfoRepository.GetCities();
            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

            return Ok(results);
        }

        [Route("api/cities/{id}")]
        public IActionResult GetCity(int Id, bool includePointsOfInterest = false)
        {
            // return new JsonResult(CitiesDataStore.Current.Cities.Where(e => e.Id == Id));

            var city = _cityInfoRepository.GetCity(Id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                //var cityResult = new CityDto
                //{
                //    Id = city.Id,
                //    Name = city.Name,
                //    Description = city.Description,
                //};

                //foreach (var poi in city.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(
                //        new PointOfInterestDto()
                //        {
                //            Id = poi.Id,
                //            Name = poi.Name,
                //            Description = poi.Description
                //        });
                //}
                //return Ok(cityResult);

                var cityResult = Mapper.Map<CityDto>(city);
                return Ok(cityResult);
            }

            //var cityWithoutPointsOfInterestResult = new CityWithoutPointsOfInterestDto
            //{
            //    Id = city.Id,
            //    Description = city.Description,
            //    Name = city.Name
            //};
            //return Ok(cityWithoutPointsOfInterestResult);
            var cityWithoutPointsOfInterestResult = Mapper.Map<CityDto>(city);
            return Ok(cityWithoutPointsOfInterestResult);
        }
    }
}
