using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/appparam")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class AppParamController : Controller
    {
        // GET services/appparam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppParam>>> GetAll()
        {
            try
            {
                return Ok(await AppParamFunctions.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/appparam/ByKey?key=
        [HttpGet("ByKey")]
        public async Task<ActionResult<String>> GetByKey([FromQuery] String key)
        {
            return Ok(await AppParamFunctions.GetValue(key));
        }
    }
}