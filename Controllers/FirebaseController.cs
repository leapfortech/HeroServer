using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/firebase")]
    [ApiController]
    public class FirebaseController : Controller
    {
        // GET services/firebase/CustomToken?tokenType="StartAnonymous"
        [HttpGet("CustomToken")]
        [AllowAnonymous]
        public async Task<ActionResult<String>> GetCustomToken([FromQuery]String tokenType)
        {
            try
            {
                return Ok(await FirebaseFunctions.GetCustomToken(tokenType));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/firebase/Roles
        [HttpGet("Roles")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyDictionary<String, object>>> GetRoles([FromQuery]String authUserId)
        {
            try
            {
                return Ok(await FirebaseFunctions.GetRoles(authUserId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/firebase/Password?length=10
        [HttpGet("Password")]
        [AllowAnonymous]
        public ActionResult<String> GeneratePassword([FromQuery]String length)
        {
            try
            {
                return Ok(FirebaseFunctions.GeneratePassword(Convert.ToInt32(length)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/firebase/Roles
        [HttpPut("Roles")]
        [AllowAnonymous]
        public async Task<ActionResult> SetRoles([FromBody]FirebaseRoles roles)
        {
            try
            {
                await FirebaseFunctions.SetRoles(roles);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}