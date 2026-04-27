using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/servicewish")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class ServiceWishController : Controller
    {
        // GET services/servicewish/GetById/?id=1
        [HttpGet("GetById")]
        public async Task<ActionResult<List<ServiceWish>>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await ServiceWishFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/servicewish/AllByType/
        [HttpPost("AllByType")]
        public async Task<ActionResult<ServiceWishAllRsp>> GetAllByType([FromBody] ServiceWishAllByTypeReq req)
        {
            try
            {
                return Ok(await ServiceWishFunctions.GetAllByType(req));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/servicewish/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] ServiceWish serviceWish)
        {
            try
            {
                return Ok(await ServiceWishFunctions.Register(serviceWish));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/servicewish
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] ServiceWish serviceWish)
        {
            try
            {
                return Ok(await ServiceWishFunctions.Update(serviceWish));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}