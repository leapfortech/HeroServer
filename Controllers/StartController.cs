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

        // GET services/GetUid?type=A
        [HttpGet("GetUid")]
        [AllowAnonymous]
        public ActionResult<long> StartApp([FromQuery]String type)
        {
            try
            {
                return Ok(SecurityFunctions.GetUid(type[0]));
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