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
        [Route("api/cities")]
        public JsonResult GetCities()
        {
            return new JsonResult( CitiesDataStore.Current.Cities );
        }

        [Route("api/cities/{id}")]
        public JsonResult GetCity(int Id)
        {
            return new JsonResult(CitiesDataStore.Current.Cities.Where(e => e.Id == Id));
        }
    }
}
