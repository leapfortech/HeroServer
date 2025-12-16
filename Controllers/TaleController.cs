using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/tale")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class TaleController : Controller
    {
        // GET services/tale?id=1
        [HttpGet]
        public async Task<ActionResult<Tale>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await TaleFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/tale/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<TaleFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await TaleFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/tale/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterTaleRequest registerTaleRequest)
        {
            try
            {
                return Ok(await TaleFunctions.Register(registerTaleRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/tale
        [HttpPut]
        public async Task<ActionResult<long>> Update([FromBody] Tale tale)
        {
            try
            {
                return Ok(await TaleFunctions.Update(tale));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}