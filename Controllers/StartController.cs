using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.Extensions.Logging;

namespace HeroServer.Controllers
{
    [Route("services")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class StartController : Controller
    {
        //readonly ILogger<RootController> wsLogger;

        //public RootController(ILogger<RootController> logger)
        //{
        //    wsLogger = logger;
        //}

        // POST services/Start
        [HttpPost("Start")]
        public async Task<ActionResult<String>> Start([FromBody]String alice)
        {
            try
            {
                return Ok(await CertificateFunctions.GetSecret(alice)); //, wsLogger));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/StartApp
        [HttpPost("StartApp")]
        public async Task<ActionResult<StartResponse>> StartApp([FromBody]StartRequest request)
        {
            try
            {
                return Ok(await StartFunctions.StartApp(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/StartBoard
        [HttpPost("StartBoard")]
        public async Task<ActionResult<StartResponse>> StartBoard([FromBody]StartRequest request)
        {
            try
            {
                return Ok(await StartFunctions.StartBoard(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}