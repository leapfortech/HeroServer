using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/notification")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class NotificationController : Controller
    {
        // GET services/notification/ByWebSysUserId?webSysUserId=1
        [HttpGet("ByWebSysUserId")]
        public async Task<ActionResult<List<Notification>>> GetByWebSysUserId([FromQuery]String webSysUserId)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(HttpContext, Convert.ToInt32(webSysUserId), "Notification.ByWebSysUserId"))
                    return Unauthorized();

                return Ok(await NotificationFunctions.GetByWebSysUserId(Convert.ToInt32(webSysUserId), 50));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/notification/Lost?id=1&webSysUserId=1
        [HttpGet("Lost")]
        public async Task<ActionResult<List<Notification>>> GetLost([FromQuery]String id, [FromQuery]String webSysUserId)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(HttpContext, Convert.ToInt32(webSysUserId), "Notification.Lost"))
                    return Unauthorized();

                return Ok(await NotificationFunctions.GetLost(Convert.ToInt32(id), Convert.ToInt32(webSysUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/notification/LastInformation?webSysUserId=1&action=AppUserVerification
        [HttpGet("LastInformation")]
        public async Task<ActionResult<String>> GetLastInformation([FromQuery]String webSysUserId, [FromQuery]String action)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(HttpContext, Convert.ToInt32(webSysUserId), "Notification.LastInformation"))
                    return Unauthorized();

                return Ok(await NotificationFunctions.GetLastInformation(Convert.ToInt32(webSysUserId), action));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/notification/ByWebSysUser?webSysUserId=3
        [HttpDelete("ByWebSysUserId")]
        public async Task<ActionResult<int>> DeleteByWebSysUserId([FromQuery]String webSysUserId)
        {
            try
            {
                await NotificationFunctions.DeleteByWebSysUserId(Convert.ToInt32(webSysUserId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}