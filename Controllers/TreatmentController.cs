using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/treatment")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class TreatmentController : Controller
    {
        // GET services/treatment?id=1
        [HttpGet]
        public async Task<ActionResult<TreatmentFull>> GetFullById([FromQuery] String id, [FromQuery] String likeAppUserId)
        {
            try
            {
                return Ok(await TreatmentFunctions.GetFullById(Convert.ToInt64(id), Convert.ToInt64(likeAppUserId)));
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
                return Ok(await TreatmentFunctions.GetFullByPostId(Convert.ToInt64(postId), Convert.ToInt64(likeAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/treatment/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<TreatmentFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await TreatmentFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/treatment/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterTreatmentRequest registerTreatmentRequest)
        {
            try
            {
                return Ok(await TreatmentFunctions.Register(registerTreatmentRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/treatment
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterTreatmentRequest registerTreatmentRequest)
        {
            try
            {
                return Ok(await TreatmentFunctions.Update(registerTreatmentRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/treatment/Accept
        [HttpPut("Accept")]
        public async Task<ActionResult<bool>> Accept([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await TreatmentFunctions.Accept(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/treatment/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await TreatmentFunctions.Reject(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}