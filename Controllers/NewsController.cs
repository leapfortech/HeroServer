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
        // GET services/news/ById?id=1
        [HttpGet("ByStatus")]
        public async Task<ActionResult<List<NewsInfo>>> GetByStatus([FromQuery] String status)
        {
            return Ok(await NewsFunctions.GetByStatus(Convert.ToInt32(status)));
        }

        // POST services/news
        [HttpPost]
        public async Task<ActionResult<long>> Register([FromBody] NewsInfo newsInfo)
        {
            try
            {
                return Ok(await NewsFunctions.Register(newsInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/news/UpdateStatus?id=5&status=1
        [HttpPut("UpdateStatus")]
        public async Task<ActionResult> UpdateStatus([FromQuery] String id, [FromQuery] String status)
        {
            try
            {
                return Ok(await NewsFunctions.UpdateStatus(Convert.ToInt64(id), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}