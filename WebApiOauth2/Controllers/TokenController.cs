using MongoDB.Driver;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApiOauth2.App_Start;
using WebApiOauth2.Models;

namespace WebApiOauth2.Controllers
{
    public class TokenController : ApiController
    {
        public MongoDBContext dbcontext = new MongoDBContext();
        public IMongoCollection<TokenModel> tokenCollection;

        [AllowAnonymous]
        [Route("api/v1/Token/access")]
        [HttpGet]
        public IHttpActionResult Access()
        {
            try
            {
                tokenCollection = dbcontext.database.GetCollection<TokenModel>("Token");

                return Ok(tokenCollection.AsQueryable().FirstOrDefault().access_token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}