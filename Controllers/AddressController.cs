using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/address")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class AddressController : Controller
    {
        // GET services/address?id=1
        [HttpGet]
        public async Task<ActionResult<Address>> GetById([FromQuery]String id)
        {
            return Ok(await AddressFunctions.GetById(Convert.ToInt32(id)));
        }

        // GET services/address/ByAppUserId?appUserId=1
        [HttpGet("ByAppUserId")]
        public async Task<ActionResult<Address>> GetByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await AddressFunctions.GetByAppUserId(Convert.ToInt32(appUserId), 1));
        }

        // GET services/address/InfoByAppUserId?appUserId=1&status=1
        [HttpGet("InfoByAppUserId")]
        public async Task<ActionResult<AddressInfo>> GetInfoByAppUserId([FromQuery]String appUserId, [FromQuery] String status)
        {
            return Ok(await AddressFunctions.GetInfoByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
        }

        // POST services/address/RegisterByAppUser?appUserId=2
        [HttpPost("RegisterByAppUser")]
        public async Task<ActionResult<int>> RegisterByAppUser([FromQuery]String appUserId, [FromBody]AddressInfo addressInfo)
        {
            try
            {
                return Ok(await AddressFunctions.RegisterByAppUser(Convert.ToInt32(appUserId), addressInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/address
        [HttpPut]
        public async Task<ActionResult<int>> Update([FromBody]Address address)
        {
            try
            {
                return Ok(await AddressFunctions.Update(address));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/address/ByAppUser?appUserId=2
        [HttpPut("ByAppUser")]
        public async Task<ActionResult<int>> UpdateByAppUser([FromQuery]String appUserId, [FromBody]Address address)
        {
            try
            {
                return Ok(await AddressFunctions.UpdateByAppUser(Convert.ToInt32(appUserId), address));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}