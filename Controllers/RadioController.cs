
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/radio")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class RadioController : Controller
    {
        // GET services/radio?id=1
        [HttpGet]
        public async Task<ActionResult<Radio>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await RadioFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/radio/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<RadioFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await RadioFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/radio/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterRadioRequest registerRadioRequest)
        {
            try
            {
                return Ok(await RadioFunctions.Register(registerRadioRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/radio
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] Radio radio)
        {
            try
            {
                return Ok(await RadioFunctions.Update(radio));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}