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
        public async Task<ActionResult<HappeningFull>> GetFullById([FromQuery] String id)
        {
            try
            {
                return Ok(await HappeningFunctions.GetFullById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FullByPostId")]
        public async Task<ActionResult<TaleFull>> GetFullByPostId([FromQuery] String postId)
        {
            try
            {
                return Ok(await HappeningFunctions.GetFullByPostId(Convert.ToInt64(postId)));
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
        public async Task<ActionResult<bool>> Accept([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await HappeningFunctions.Accept(postModerationRequest.PostId, postModerationRequest.SubtypeId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/happening/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await HappeningFunctions.Reject(postModerationRequest.PostId, postModerationRequest.SubtypeId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}