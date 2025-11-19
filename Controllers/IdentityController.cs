using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/identity")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class IdentityController : Controller
    {
        // GET services/identity/All?status=1
        [HttpGet("All")]
        public async Task<ActionResult<List<Identity>>> GetAll([FromQuery]String status = "-1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetAll(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/FullAll?status=1
        [HttpGet("FullAll")]
        public async Task<ActionResult<List<IdentityFull>>> GetFullAll([FromQuery] String status = "-1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetFullAll(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<Identity>> GetById([FromQuery]String id)
        {
            try
            {
                return Ok(await IdentityFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/ByAppUserId?appUserId=1&status=1
        [HttpGet("ByAppUserId")]
        public async Task<ActionResult<Identity>> GetByAppUserId([FromQuery]String appUserId, [FromQuery]String status = "1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetByAppUserId(Convert.ToInt64(appUserId), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/FullByAppUserId?appUserId=1&status=1
        [HttpGet("FullByAppUserId")]
        public async Task<ActionResult<IdentityFull>> GetFullByAppUserId([FromQuery]String appUserId, [FromQuery]String status = "1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetFullByAppUserId(Convert.ToInt64(appUserId), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/PortraitByAppUserId?appUserId=1
        [HttpGet("PortraitByAppUserId")]
        public async Task<ActionResult<String>> GetPortraitByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await IdentityFunctions.GetPortraitByAppUserId(Convert.ToInt64(appUserId)));
        }

        // GET services/identity/AllByAppUserId?appUserId=1&status=0
        [HttpGet("AllByAppUserId")]
        public async Task<ActionResult<List<Identity>>> GetAllByAppUserId([FromQuery]String appUserId, [FromQuery]String status = "1")
        {
            return Ok(await IdentityFunctions.GetAllByAppUserId(Convert.ToInt64(appUserId), Convert.ToInt32(status)));
        }

        // POST services/identity/Register
        [HttpPost("Register")]
        public async Task<ActionResult<int>> Register([FromBody]IdentityRegister identityRegister)
        {
            try
            {
                return Ok(await IdentityFunctions.Register(identityRegister));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/identity/
        [HttpPost]
        public async Task<ActionResult<long>> Add([FromBody]Identity identity)
        {
            try
            {
                return Ok(await IdentityFunctions.Add(identity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/
        [HttpPut]
        public async Task<ActionResult<long>> Update([FromBody]Identity identity)
        {
            try
            {
                return Ok(await IdentityFunctions.Update(identity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/Portrait?appUserId=3
        [HttpPut("Portrait")]
        public async Task<ActionResult> UpdatePortrait([FromQuery]String appUserId, [FromBody]String portrait)
        {
            try
            {
                await IdentityFunctions.UpdatePortrait(Convert.ToInt64(appUserId), portrait);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/Status?id=5&status=1
        [HttpPut("Status")]
        public async Task<ActionResult> UpdateStatus([FromQuery]String id, [FromQuery]String status)
        {
            try
            {
                if (!await IdentityFunctions.UpdateStatus(Convert.ToInt64(id), Convert.ToInt32(status)))
                    return BadRequest("[STAT]");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
