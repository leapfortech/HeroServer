using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/appUser")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class AppUserController : Controller
    {
        // GET services/appUser/Named?count=1&page=1
        [HttpGet("Named")]
        public async Task<ActionResult<List<AppUserNamed>>> GetNamed([FromQuery]String count = "0", [FromQuery]String page = "0")
        {
            try
            {
                return Ok(await AppUserFunctions.GetNamed(Convert.ToInt32(count), Convert.ToInt32(page)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/appUser/NamedByStatus?status=0&count=1&page=1
        [HttpGet("NamedByStatus")]
        public async Task<ActionResult<List<AppUserNamed>>> GetNamedByStatus([FromQuery]String status, [FromQuery]String count = "0", [FromQuery]String page = "0")
        {
            try
            {
                return Ok(await AppUserFunctions.GetNamedByStatus(Convert.ToInt32(status), Convert.ToInt32(count), Convert.ToInt32(page)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/appUser/FullByStatus?status=1
        [HttpGet("FullByStatus")]
        public async Task<ActionResult<List<AppUserNamed>>> GetFullByStatus([FromQuery]String status = "1")
        {
            try
            {
                return Ok(await AppUserFunctions.GetFullByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/appUser/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<AppUser>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await AppUserFunctions.GetById(Convert.ToInt32(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/appUser/Count
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetCountAll()
        {
            try
            {
                return Ok(await AppUserFunctions.GetCountAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/appUser/CountByStatus?status=0
        [HttpGet("CountByStatus")]
        public async Task<ActionResult<int>> GetCountByStatus([FromQuery]String status)
        {
            try
            {
                return Ok(await AppUserFunctions.GetCountByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/appUser
        [HttpPut]
        public async Task<ActionResult> Update([FromBody]AppUser appUser)
        {
            try
            {
                await AppUserFunctions.Update(appUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/appUser/UpdatePhone?id=1&options=1
        [HttpPut("UpdatePhone")]
        public async Task<ActionResult> UpdatePhone([FromBody]PhoneRequest phoneRequest)
        {
            try
            {
                //if (!await FirebaseFunctions.AuthorizeAppUser(blcConnString, HttpContext, Convert.ToInt32(id), "AppUser.UpdateOptions"))
                //    return Unauthorized();

                await AppUserFunctions.UpdatePhone(phoneRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/appUser/UpdateOptions?id=1&options=1
        [HttpPut("UpdateOptions")]
        public async Task<ActionResult> UpdateOptions([FromQuery]String id, [FromQuery]String options)
        {
            try
            {
                //if (!await FirebaseFunctions.AuthorizeAppUser(blcConnString, HttpContext, Convert.ToInt32(id), "AppUser.UpdateOptions"))
                //    return Unauthorized();

                if (!await AppUserFunctions.UpdateOptions(Convert.ToInt32(id), Convert.ToInt32(options)))
                    return BadRequest($"[OPTI]");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/appUser/UpdateStatus?id=1&appUserStatusId=1
        [HttpPut("UpdateStatus")]
        public async Task<ActionResult> UpdateStatus([FromQuery]String id, [FromQuery]String appUserStatusId)
        {
            try
            {
                //if (!await FirebaseFunctions.AuthorizeAppUser(bntConnString, HttpContext, Convert.ToInt32(id), "AppUser.SetStatus"))
                //    return Unauthorized();

                if (!await AppUserFunctions.UpdateStatus(Convert.ToInt32(id), Convert.ToInt32(appUserStatusId)))
                    return BadRequest("[STAT]");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/appUser/UpdateReferred?id=1&referredCode=xx
        [HttpPut("UpdateReferred")]
        public async Task<ActionResult<int>> UpdateReferred([FromQuery]String id, [FromQuery]String referredCode)
        {
            try
            {
                return await AppUserFunctions.UpdateReferred(Convert.ToInt32(id), referredCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/appUser/ById?id=3&authUser=1&renap=0
        [HttpDelete("ById")]
        public async Task<ActionResult<int>> DeleteById([FromQuery]String id, [FromQuery]String authUser = "1")
        {
            try
            {
                await AppUserFunctions.DeleteById(Convert.ToInt32(id), authUser == "1");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/appUser/ByEmail?email=user@gmail.com&authUser=0&renap=1
        [HttpDelete("ByEmail")]
        public async Task<ActionResult<int>> DeleteByEmail([FromQuery]String email, [FromQuery]String authUser = "1", [FromQuery]String renap = "0")
        {
            try
            {
                await AppUserFunctions.DeleteByEmail(email, authUser == "1", renap == "1");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}