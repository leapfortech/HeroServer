using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/ive")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    
    public class IveController : Controller
    {
        // POST services/ive
        [HttpPost]
        public async Task<ActionResult<Formulario>> Ive([FromBody]IveRequest iveRequest)
        {
            try
            {
                return Ok(await IveFunctions.GetIve(iveRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}