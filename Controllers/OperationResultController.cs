using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/operationresult")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class OperationResultController : Controller
    {
        // GET services/operationresult/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<OperationResult>> GetById([FromQuery] String id)
        {
            return Ok(await OperationResultFunctions.GetById(Convert.ToInt32(id)));
        }

        // GET services/operationresult/ByProjectId?projectId=1
        [HttpGet("ByProjectId")]
        public async Task<ActionResult<IEnumerable<OperationResult>>> GetByProjectId([FromQuery] String projectId, [FromQuery] String createDateTime)
        {
            return Ok(await OperationResultFunctions.GetByProjectId(Convert.ToInt32(projectId), Convert.ToDateTime(createDateTime)));
        }

        // POST services/operationresult
        [HttpPost]
        public async Task<ActionResult<int>> Register([FromBody] OperationResult operationResult)
        {
            try
            {
                return Ok(await OperationResultFunctions.Register(operationResult));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/operationresult
        [HttpPut]
        public async Task<ActionResult<int>> Update([FromBody] OperationResult operationResult)
        {
            try
            {
                return Ok(await OperationResultFunctions.Update(operationResult));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}