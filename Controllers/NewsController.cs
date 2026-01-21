using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/news")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class NewsController : Controller
    {
        // GET services/news?id=1
        [HttpGet]
        public async Task<ActionResult<News>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await NewsFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/news/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<NewsFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await NewsFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/news/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterNewsRequest registerNewsRequest)
        {
            try
            {
                return Ok(await NewsFunctions.Register(registerNewsRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/news
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterNewsRequest registerNewsRequest)
        {
            try
            {
                return Ok(await NewsFunctions.Update(registerNewsRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}