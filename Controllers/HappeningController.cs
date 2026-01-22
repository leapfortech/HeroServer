using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/happening")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class HappeningController : Controller
    {
        // GET services/happening?id=1
        [HttpGet]
        public async Task<ActionResult<Happening>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await HappeningFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/happening/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<HappeningFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await HappeningFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/happening/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterHappeningRequest registerHappeningRequest)
        {
            try
            {
                return Ok(await HappeningFunctions.Register(registerHappeningRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/happening
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterHappeningRequest registerHappeningRequest)
        {
            try
            {
                return Ok(await HappeningFunctions.Update(registerHappeningRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/happening/Accept
        [HttpPut("Accept")]
        public async Task<ActionResult<bool>> Accept([FromQuery] String postId, [FromQuery] String happeningId)
        {
            try
            {
                return Ok(await HappeningFunctions.Accept(Convert.ToInt64(postId), Convert.ToInt64(happeningId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/happening/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromQuery] String postId, [FromQuery] String happeningId)
        {
            try
            {
                return Ok(await HappeningFunctions.Reject(Convert.ToInt64(postId), Convert.ToInt64(happeningId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}