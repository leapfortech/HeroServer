using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/genvalues")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class GenValuesController : Controller
    {
        // POST services/genvalues
        [HttpPost]
        public async Task<ActionResult<GenValues>> Get([FromBody]GenValuesParams valuesParams)
        {
            try
            {
                if (valuesParams.TableName == null || valuesParams.TableName.Length == 0 || valuesParams.TableName[0] != 'K')
                    return BadRequest();
                return Ok(await new GenValuesDB().Get(valuesParams));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/genvalues/All
        [HttpPost("All")]
        public async Task<ActionResult<GenValuesList>> GetAll([FromBody]GenValuesParams[] valuesParams)
        {
            try
            {
                return Ok(await new GenValuesDB().Get(valuesParams));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/genvalues
        //[HttpPut]
        //public async Task<ActionResult<GenValues>> Set([FromBody]GenValuesParams valuesParams)
        //{
        //    return Ok(await new GenValuesDB(bntConnString).Set(valuesParams));
        //}

        // GET services/genvalues/NameByCode?table=K-Country&code=FRA
        //[HttpGet("NameByCode")]
        //public async Task<ActionResult<GenValues>> GetNameByCode([FromQuery]String table, [FromQuery]String code)
        //{
        //    return Ok(await GenValuesFunctions.GetNameByCode(bntConnString, table, code));
        //}
    }
}
