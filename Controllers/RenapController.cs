using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/renap")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    
    public class RenapController : Controller
    {
        // GET services/renap/Identity?cui=1992456890101
        [HttpGet("Identity")]
        public async Task<ActionResult<RenapIdentity>> GetIdentityByCui([FromQuery]String cui)
        {
            try
            {
                return Ok(await RenapFunctions.GetIdentityByCui(cui));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/renap/IdentityInfoByCuiByCui?cui=1992456890101
        [HttpGet("IdentityInfoByCui")]
        public async Task<ActionResult<RenapIdentityInfo>> GetIdentityInfoByCui([FromQuery] String cui)
        {
            try
            {
                return Ok(await RenapFunctions.GetIdentityInfoByCui(cui));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/renap/IdentityInfo?appUserId=1
        [HttpGet("IdentityInfo")]
        public async Task<ActionResult<RenapIdentityInfo>> GetIdentityInfo([FromQuery] String appUserId)
        {
            try
            {
                return Ok(await RenapFunctions.GetIdentityInfo(Convert.ToInt32(appUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST services/renap/CompareNames
        [HttpPost("CompareNames")]
        public async Task<ActionResult<LeapResponse>> CompareNames([FromBody]RenapRequest renapRequest)
        {
            try
            {
                return Ok(await RenapFunctions.CompareNames(renapRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/renap/CompareIdentity
        [HttpPost("CompareIdentity")]
        public async Task<ActionResult<LeapResponse>> CompareIdentity([FromBody]RenapRequest renapRequest)
        {
            try
            {
                return Ok(await RenapFunctions.CompareIdentity(renapRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/renap/CompareFace
        [HttpPost("CompareFace")]
        public async Task<ActionResult<LeapResponse>> CompareFace([FromBody]RenapRequest renapRequest)
        {
            try
            {
                return Ok(await RenapFunctions.CompareFace(renapRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}