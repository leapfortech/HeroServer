using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/puzzle")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class PuzzleController : Controller
    {
        // GET services/puzzle?id=1
        [HttpGet]
        public async Task<ActionResult<PuzzleFull>> GetFullById([FromQuery] String id, [FromQuery] String likeAppUserId)
        {
            try
            {
                return Ok(await PuzzleFunctions.GetFullById(Convert.ToInt64(id), Convert.ToInt64(likeAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/puzzle/FullByPostId?postId=1, likeAppUserId=1
        [HttpGet("FullByPostId")]
        public async Task<ActionResult<PuzzleFull>> GetFullByPostId([FromQuery] String postId, [FromQuery] String likeAppUserId)
        {
            try
            {
                return Ok(await PuzzleFunctions.GetFullByPostId(Convert.ToInt64(postId), Convert.ToInt64(likeAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/puzzle/AllByDifficulty/
        [HttpPost("AllByDifficulty")]
        public async Task<ActionResult<PuzzleAllRsp>> GetAllByDifficulty([FromBody] PuzzleAllByDifficultyReq req)
        {
            try
            {
                return Ok(await PuzzleFunctions.GetAllByDifficulty(req));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/puzzle/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<PuzzleFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PuzzleFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/puzzle/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterPuzzleRequest registerPuzzleRequest)
        {
            try
            {
                return Ok(await PuzzleFunctions.Register(registerPuzzleRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/puzzle
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterPuzzleRequest registerPuzzleRequest)
        {
            try
            {
                return Ok(await PuzzleFunctions.Update(registerPuzzleRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/puzzle/Accept
        [HttpPut("Accept")]
        public async Task<ActionResult<bool>> Accept([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await PuzzleFunctions.Accept(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/puzzle/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await PuzzleFunctions.Reject(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/puzzle/UpdateStatus
        [HttpPut("UpdateStatus")]
        public async Task<ActionResult<bool>> UpdateStatus([FromQuery] String postId, [FromQuery] String puzzleId, [FromQuery] String status)
        {
            try
            {
                return Ok(await PuzzleFunctions.UpdateStatus(Convert.ToInt64(postId), Convert.ToInt64(puzzleId), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}