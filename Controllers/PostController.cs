using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/post")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class PostController : Controller
    {
        // GET services/post?id=1
        [HttpGet]
        public async Task<ActionResult<Post>> GetById([FromQuery]String id)
        {
            try
            {
                return Ok(await PostFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/post/ImagesById?id=1&first=true
        [HttpGet("ImagesById")]
        public async Task<ActionResult<List<String>>> GetImagesById([FromQuery] String id, [FromQuery] String first = "true")
        {
            try
            {
                return Ok(await PostFunctions.GetImagesById(Convert.ToInt64(id), first == "true"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/Feed
        [HttpPost("Feed")]
        public async Task<ActionResult<PostFeedResponse>> GetPostFeed([FromBody] PostFeedRequest request)
        {
            try
            {
                return Ok(await PostFunctions.GetPostFeed(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterShare
        [HttpPost("RegisterShare")]
        public async Task<ActionResult<long>> RegisterShare([FromBody] Share share)
        {
            try
            {
                return Ok(await PostFunctions.RegisterShare(share));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterFavorite
        [HttpPost("RegisterFavorite")]
        public async Task<ActionResult<long>> RegisterFavorite([FromBody] Favorite favorite)
        {
            try
            {
                return Ok(await PostFunctions.RegisterFavorite(favorite));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterComment
        [HttpPost("RegisterComment")]
        public async Task<ActionResult<long>> RegisterComment([FromBody] Comment comment)
        {
            try
            {
                return Ok(await PostFunctions.RegisterComment(comment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterCommentPlaint
        [HttpPost("RegisterCommentPlaint")]
        public async Task<ActionResult<long>> RegisterCommentPlaint([FromBody] CommentPlaint commentPlaint)
        {
            try
            {
                return Ok(await PostFunctions.RegisterCommentPlaint(commentPlaint));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterPostPlaint
        [HttpPost("RegisterPostPlaint")]
        public async Task<ActionResult<long>> RegisterPostPlaint([FromBody] PostPlaint postPlaint)
        {
            try
            {
                return Ok(await PostFunctions.RegisterPostPlaint(postPlaint));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterPostRead
        [HttpPost("RegisterPostRead")]
        public async Task<ActionResult<long>> RegisterPostRead([FromBody] PostRead postRead)
        {
            try
            {
                return Ok(await PostFunctions.RegisterPostRead(postRead));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterReaction
        [HttpPost("RegisterReaction")]
        public async Task<ActionResult<long>> RegisterReaction([FromBody] Reaction reaction)
        {
            try
            {
                return Ok(await PostFunctions.RegisterReaction(reaction));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterLike
        [HttpPost("RegisterLike")]
        public async Task<ActionResult<long>> RegisterLike([FromBody] Like like)
        {
            try
            {
                return Ok(await PostFunctions.RegisterLike(like));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/post
        [HttpPut]
        public async Task<ActionResult<long>> Update([FromBody]Post post)
        {
            try
            {
                return Ok(await PostFunctions.Update(post));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/post/ById?id=3
        [HttpDelete("ById")]
        public async Task<ActionResult<long>> DeleteById([FromQuery] String id)
        {
            try
            {
                await PostFunctions.DeleteById(Convert.ToInt64(id));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}