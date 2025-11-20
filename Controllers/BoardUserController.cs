using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/boardUser")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class BoardUserController : Controller
    {
        // GET services/appUser/All
        [HttpGet()]
        public async Task<ActionResult<List<BoardUser>>> GetAll()
        {
            try
            {
                return Ok(await BoardUserFunctions.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/appUser/Fulls
        [HttpGet("Fulls")]
        public async Task<ActionResult<List<BoardUser>>> GetFulls()
        {
            try
            {
                return Ok(await BoardUserFunctions.GetFulls());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/boardUser/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<BoardUser>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await BoardUserFunctions.GetById(Convert.ToInt32(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/boardUser/Count
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetCountAll()
        {
            try
            {
                return Ok(await BoardUserFunctions.GetCountAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/boardUser/CountByStatus?status=0
        [HttpGet("CountByStatus")]
        public async Task<ActionResult<int>> GetCountByStatus([FromQuery]String status)
        {
            try
            {
                return Ok(await BoardUserFunctions.GetCountByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/boardUser/Full
        [HttpPut("Full")]
        public async Task<ActionResult> UpdateFull([FromBody]BoardUserFull boardUserFull)
        {
            try
            {
                await BoardUserFunctions.UpdateFull(boardUserFull);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/boardUser
        [HttpPut]
        public async Task<ActionResult> Update([FromBody]BoardUser boardUser)
        {
            try
            {
                await BoardUserFunctions.Update(boardUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/boarduser/UpdateStatus?id=1&status=1
        [HttpPut("UpdateStatus")]
        public async Task<ActionResult> UpdateStatus([FromQuery]String id, [FromQuery]String status)
        {
            try
            {
                //if (!await FirebaseFunctions.AuthorizeAppUser(bntConnString, HttpContext, Convert.ToInt32(id), "AppUser.SetStatus"))
                //    return Unauthorized();

                if (!await BoardUserFunctions.UpdateStatus(Convert.ToInt32(id), Convert.ToInt32(status)))
                    return BadRequest("[STAT]");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/boarduser/ById?id=3
        [HttpDelete("ById")]
        public async Task<ActionResult<long>> DeleteById([FromQuery]String id, [FromQuery]String authUser = "1")
        {
            try
            {
                await BoardUserFunctions.DeleteById(Convert.ToInt64(id), authUser == "1");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/boarduser/ByEmail?email=user@gmail.com&authUser=0
        [HttpDelete("ByEmail")]
        public async Task<ActionResult<long>> DeleteByEmail([FromQuery]String email, [FromQuery]String authUser = "1")
        {
            try
            {
                await BoardUserFunctions.DeleteByEmail(email, authUser == "1");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}