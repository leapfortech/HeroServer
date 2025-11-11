using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/ofac")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    
    public class OfacController : Controller
    {

        // POST services/ofac/Check
        [HttpPost("Check")]
        public async Task<ActionResult<LeapResponse>> Check([FromBody]OfacRequest ofacRequest)
        {
            try
            {
                return Ok(await OfacFunctions.Check(ofacRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/ofac/Find
        [HttpPost("Find")]
        public async Task<ActionResult<LeapResponse>> Find([FromBody]OfacRequest ofacRequest)
        {
            try
            {
                return Ok(await OfacFunctions.Find(ofacRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}