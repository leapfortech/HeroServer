using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/recipe")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class RecipeController : Controller
    {
        // GET services/recipe?id=1
        [HttpGet]
        public async Task<ActionResult<RecipeFull>> GetFullById([FromQuery] String id, [FromQuery] long likeAppUserId)
        {
            try
            {
                return Ok(await RecipeFunctions.GetFullById(Convert.ToInt64(id), Convert.ToInt64(likeAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FullByPostId")]
        public async Task<ActionResult<TaleFull>> GetFullByPostId([FromQuery] String postId, [FromQuery] long likeAppUserId)
        {
            try
            {
                return Ok(await RecipeFunctions.GetFullByPostId(Convert.ToInt64(postId), Convert.ToInt64(likeAppUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/recipe/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<RecipeFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await RecipeFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/recipe/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterRecipeRequest registerRecipeRequest)
        {
            try
            {
                return Ok(await RecipeFunctions.Register(registerRecipeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/recipe
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterRecipeRequest registerRecipeRequest)
        {
            try
            {
                return Ok(await RecipeFunctions.Update(registerRecipeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/recipe/Accept
        [HttpPut("Accept")]
        public async Task<ActionResult<bool>> Accept([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await RecipeFunctions.Accept(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/recipe/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await RecipeFunctions.Reject(postModerationRequest.PostId, postModerationRequest.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}