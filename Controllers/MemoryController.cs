using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/memory")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class MemoryController : Controller
    {
        // GET services/memory?id=1
        [HttpGet]
        public async Task<ActionResult<MemoryFull>> GetFullById([FromQuery] String id, [FromQuery] String reactionAppUserId)
        {
            try
            {
                return Ok(await MemoryFunctions.GetFullById(Convert.ToInt64(id), Convert.ToInt64(reactionAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FullByPostId")]
        public async Task<ActionResult<MemoryFull>> GetFullByPostId([FromQuery] String postId, [FromQuery] String reactionAppUserId)
        {
            try
            {
                return Ok(await MemoryFunctions.GetFullByPostId(Convert.ToInt64(postId), Convert.ToInt64(reactionAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/memory/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<MemoryFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await MemoryFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/memory/Feed
        [HttpPost("Feed")]
        public async Task<ActionResult<MemoryFeedResponse>> GetFeed([FromBody] MemoryFeedRequest request)
        {
            try
            {
                return Ok(await MemoryFunctions.GetFeed(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/memory/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] MemoryFull memoryFull)
        {
            try
            {
                return Ok(await MemoryFunctions.Register(memoryFull));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/memory
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] MemoryFull memoryFull)
        {
            try
            {
                return Ok(await MemoryFunctions.Update(memoryFull));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/memory/Accept
        [HttpPut("Accept")]
        public async Task<ActionResult<bool>> Accept([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await MemoryFunctions.Accept(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/memory/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await MemoryFunctions.Reject(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}