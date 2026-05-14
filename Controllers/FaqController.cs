using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/faq")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class FaqController : Controller
    {
        // GET services/faq/GetById/?id=1
        [HttpGet("GetById")]
        public async Task<ActionResult<List<Faq>>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await FaqFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/faq/ByType/
        [HttpPost("ByType")]
        public async Task<ActionResult<List<Faq>>> GetByType([FromQuery] long faqTypeId)
        {
            try
            {
                return Ok(await FaqFunctions.GetByType(faqTypeId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/faq/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] Faq faq)
        {
            try
            {
                return Ok(await FaqFunctions.Register(faq));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/faq
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] Faq faq)
        {
            try
            {
                return Ok(await FaqFunctions.Update(faq));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/UpdateStatus
        [HttpPut("UpdateStatus")]
        public async Task<ActionResult<bool>> UpdateStatus([FromQuery] long id, [FromQuery] int status)
        {
            try
            {
                return Ok(await FaqFunctions.UpdateStatus(id, status));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}