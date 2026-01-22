using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/product")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class ProductController : Controller
    {
        // GET services/product?id=1
        [HttpGet]
        public async Task<ActionResult<Product>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await ProductFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/product/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<ProductFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await ProductFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/product/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterProductRequest registerProductRequest)
        {
            try
            {
                return Ok(await ProductFunctions.Register(registerProductRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/product/RegisterReview
        [HttpPost("RegisterReview")]
        public async Task<ActionResult<long>> RegisterReview([FromBody] ProductReview productReview)
        {
            try
            {
                return Ok(await ProductFunctions.RegisterReview(productReview));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/product
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterProductRequest registerProductRequest)
        {
            try
            {
                return Ok(await ProductFunctions.Update(registerProductRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/product/Accept
        [HttpPut("Accept")]
        public async Task<ActionResult<bool>> Accept([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await ProductFunctions.Accept(postModerationRequest.PostId, postModerationRequest.SubtypeId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/product/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] PostModerationRequest postModerationRequest)
        {
            try
            {
                return Ok(await ProductFunctions.Reject(postModerationRequest.PostId, postModerationRequest.SubtypeId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}