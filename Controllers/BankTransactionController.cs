using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/banktransaction")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class BankTransactionController : Controller
    {

        // GET services/banktransaction/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<BankTransaction>> GetById([FromQuery]String id)
        {
            return Ok(await BankTransactionFunctions.GetById(Convert.ToInt32(id)));
        }

        // GET services/banktransaction/ByAppUserId?appUserId=1
        [HttpGet("ByAppUserId")]
        public async Task<ActionResult<List<BankTransaction>>> GetByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await BankTransactionFunctions.GetByAppUserId(Convert.ToInt32(appUserId)));
        }

        // GET services/banktransaction/Receipt?id=1
        [HttpGet("Receipt")]
        public async Task<ActionResult<String>> GetReceipt([FromQuery]String id)
        {
            return Ok(await BankTransactionFunctions.GetReceipt(Convert.ToInt32(id)));
        }

        // POST services/banktransaction/UpdateStatus?id=1&status=1
        [HttpPut("UpdateStatus")]
        public async Task<ActionResult> UpdateStatus([FromQuery]String id, [FromQuery]String status)
        {
            try
            {
                if (!await BankTransactionFunctions.UpdateStatus(Convert.ToInt32(id), Convert.ToInt32(status)))
                    return BadRequest("[STAT]");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}