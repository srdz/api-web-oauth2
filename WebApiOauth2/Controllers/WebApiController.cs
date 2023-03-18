using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiOauth2.App_Start;
using WebApiOauth2.Models;

namespace WebApiOauth2.Controllers
{
    [Authorize]
    public class WebApiController : ApiController
    {
        // GET api/WebApi
        public IEnumerable<string> Get()
        {
            return new string[] { "Authorization granted" };
        }
    }
}