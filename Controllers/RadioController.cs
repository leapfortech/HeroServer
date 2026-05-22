
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/radio")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class RadioController : Controller
    {
        // GET services/radio?id=1
        [HttpGet]
        public async Task<ActionResult<RadioFull>> GetFullById([FromQuery] String id, [FromQuery] String likeAppUserId)
        {
            try
            {
                return Ok(await RadioFunctions.GetFullById(Convert.ToInt64(id), Convert.ToInt64(likeAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FullByPostId")]
        public async Task<ActionResult<TaleFull>> GetFullByPostId([FromQuery] String postId, [FromQuery] String likeAppUserId)
        {
            try
            {
                return Ok(await RadioFunctions.GetFullByPostId(Convert.ToInt64(postId), Convert.ToInt64(likeAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/radio/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<RadioFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await RadioFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/radio/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterRadioRequest registerRadioRequest)
        {
            try
            {
                return Ok(await RadioFunctions.Register(registerRadioRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/radio/RegisterRadioListen
        [HttpPost("RegisterRadioListen")]
        public async Task<ActionResult<long>> RegisterRadioListen([FromBody] RadioListen radioListen)
        {
            try
            {
                return Ok(await RadioFunctions.RegisterRadioListen(radioListen));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/radio
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterRadioRequest registerRadioRequest)
        {
            try
            {
                return Ok(await RadioFunctions.Update(registerRadioRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/radio/Accept
        [HttpPut("Accept")]
        public async Task<ActionResult<bool>> Accept([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await RadioFunctions.Accept(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/radio/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await RadioFunctions.Reject(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}