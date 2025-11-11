using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace HeroServer.Controllers
{
    [Route("services/referred")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class ReferredController(ILogger<ReferredController> logger) : Controller
    {
        readonly ILogger<ReferredController> wsLogger = logger;

        // GET services/referred/All
        [HttpGet("All")]
        public async Task<ActionResult<List<Referred>>> GetAll()
        {
            return Ok(await ReferredFunctions.GetAll());
        }

        // GET services/referred/FullAll
        [HttpGet("FullAll")]
        public async Task<ActionResult<List<ReferredFull>>> GetFullAll()
        {
            try
            {
                return Ok(await ReferredFunctions.GetFullAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/referred/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<Referred>> GetById([FromQuery]String id)
        {
            return Ok(await ReferredFunctions.GetById(Convert.ToInt32(id)));
        }

        // GET services/referred/ByAppUserId?appUserId=1
        [HttpGet("ByAppUserId")]
        public async Task<ActionResult<IEnumerable<Referred>>> GetByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await ReferredFunctions.GetByAppUserId(Convert.ToInt32(appUserId)));
        }

        // GET services/referred/IdByCode?code=
        [HttpGet("IdByCode")]
        public async Task<ActionResult<int>> GetIdByCode([FromQuery] String code)
        {
            return Ok(await ReferredFunctions.GetIdByCode(code));
        }

        // GET services/referred/Validate?code=
        [HttpGet("Validate")]     // JAD : Remove
        public async Task<ActionResult<int>> Validate([FromQuery] String code)
        {
            return Ok(await ReferredFunctions.Validate(code));
        }

        // POST services/referred/History
        [HttpPost("History")]
        public async Task<ActionResult<IEnumerable<Referred>>> GetHistory([FromBody] ReferredHistoryRequest referredHistoryRequest)
        {
            return Ok(await ReferredFunctions.GetHistory(referredHistoryRequest));
        }

        // POST services/referred/ByPeriod
        [HttpPost("ByPeriod")]     // JAD : Remove
        public async Task<ActionResult<IEnumerable<Referred>>> GetByPeriod([FromBody] ReferredHistoryRequest referredHistoryRequest)
        {
            return Ok(await ReferredFunctions.GetHistory(referredHistoryRequest));
        }

        // POST services/referred/register
        [HttpPost("Register")]
        public async Task<ActionResult<String>> Register([FromBody] Referred referred)
        {
            try
            {
                return Ok(await ReferredFunctions.Register(referred, wsLogger));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/referred
        [HttpPut]
        public async Task<ActionResult<int>> Update([FromBody]Referred referred)
        {
            try
            {
                return Ok(await ReferredFunctions.Update(referred));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}