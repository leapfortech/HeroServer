using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/identity")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class IdentityController : Controller
    {
        // GET services/identity/All?status=1
        [HttpGet("All")]
        public async Task<ActionResult<List<Identity>>> GetAll([FromQuery]String status = "-1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetAll(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/FullAll?status=1
        [HttpGet("FullAll")]
        public async Task<ActionResult<List<IdentityFull>>> GetFullAll([FromQuery] String status = "-1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetFullAll(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<Identity>> GetById([FromQuery]String id)
        {
            try
            {
                return Ok(await IdentityFunctions.GetById(Convert.ToInt32(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/ByAppUserId?appUserId=1&status=1
        [HttpGet("ByAppUserId")]
        public async Task<ActionResult<Identity>> GetByAppUserId([FromQuery]String appUserId, [FromQuery]String status = "1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/FullByAppUserId?appUserId=1&status=1
        [HttpGet("FullByAppUserId")]
        public async Task<ActionResult<IdentityFull>> GetFullByAppUserId([FromQuery]String appUserId, [FromQuery]String status = "1")
        {
            try
            {
                return Ok(await IdentityFunctions.GetFullByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/identity/InfoByAppUserId?appUserId=1&status=1
        [HttpGet("InfoByAppUserId")]
        public async Task<ActionResult<IdentityInfo>> GetInfoByAppUserId([FromQuery]String appUserId, [FromQuery]String status = "1")
        {
            return Ok(await IdentityFunctions.GetInfoByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
        }

        // GET services/identity/BoardInfoByAppUserId?appUserId=1&status=1
        [HttpGet("BoardInfoByAppUserId")]
        public async Task<ActionResult<IdentityInfo>> GetBoardInfoByAppUserId([FromQuery] String appUserId, [FromQuery] String status = "1")
        {
            return Ok(await IdentityFunctions.GetBoardInfoByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
        }

        // GET services/identity/DpiPhotoByAppUserId?appUserId=1
        [HttpGet("DpiPhotoByAppUserId")]
        public async Task<ActionResult<DpiPhoto>> GetDpiPhotoByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await IdentityFunctions.GetDpiPhoto(Convert.ToInt32(appUserId)));
        }

        // GET services/identity/DpiBoardPhotoByAppUserId?appUserId=1
        [HttpGet("DpiBoardPhotoByAppUserId")]
        public async Task<ActionResult<DpiPhoto>> GetDpiBoardPhotoByAppUserId([FromQuery] String appUserId)
        {
            return Ok(await IdentityFunctions.GetDpiBoardPhoto(Convert.ToInt32(appUserId)));
        }

        // GET services/identity/PortraitByAppUserId?appUserId=1
        [HttpGet("PortraitByAppUserId")]
        public async Task<ActionResult<String>> GetPortraitByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await IdentityFunctions.GetPortraitByAppUserId(Convert.ToInt32(appUserId)));
        }

        // GET services/identity/AllByAppUserId?appUserId=1&status=0
        [HttpGet("AllByAppUserId")]
        public async Task<ActionResult<List<Identity>>> GetAllByAppUserId([FromQuery]String appUserId, [FromQuery]String status = "1")
        {
            return Ok(await IdentityFunctions.GetAllByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
        }

        // GET services/identity/Signature?signatureId=1
        [HttpGet("Signature")]
        public async Task<ActionResult<String>> GetSignature([FromQuery] String signatureId)
        {
            return Ok(await IdentityFunctions.GetSignature(Convert.ToInt32(signatureId)));
        }

        // POST services/identity/Register
        [HttpPost("Register")]
        public async Task<ActionResult<int>> Register([FromBody]IdentityRegister identityRegister)
        {
            try
            {
                return Ok(await IdentityFunctions.Register(identityRegister));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/identity/
        [HttpPost]
        public async Task<ActionResult<int>> Add([FromBody]Identity identity)
        {
            try
            {
                return Ok(await IdentityFunctions.Add(identity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/
        [HttpPut]
        public async Task<ActionResult<int>> Update([FromBody]Identity identity)
        {
            try
            {
                return Ok(await IdentityFunctions.Update(identity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/DpiFront?appUserId=3
        [HttpPut("DpiFront")]
        public async Task<ActionResult> UpdateDpiFront([FromQuery]String appUserId, [FromBody]String dpiPhotos)
        {
            try
            {
                
                await IdentityFunctions.UpdateDpiFront(Convert.ToInt32(appUserId), dpiPhotos);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/DpiBack?appUserId=3
        [HttpPut("DpiBack")]
        public async Task<ActionResult> UpdateDpiBack([FromQuery]String appUserId, [FromBody]String dpiBack)
        {
            try
            {
                await IdentityFunctions.UpdateDpiBack(Convert.ToInt32(appUserId), dpiBack);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/Portrait?appUserId=3
        [HttpPut("Portrait")]
        public async Task<ActionResult> UpdatePortrait([FromQuery]String appUserId, [FromBody]String portrait)
        {
            try
            {
                await IdentityFunctions.UpdatePortrait(Convert.ToInt32(appUserId), portrait);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/Info
        [HttpPut("Info")]
        public async Task<ActionResult<int>> UpdateInfo([FromBody]IdentityInfo identityInfo)
        {
            try
            {
                return Ok(await IdentityFunctions.UpdateInfo(identityInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/identity/Status?id=5&status=1
        [HttpPut("Status")]
        public async Task<ActionResult> UpdateStatus([FromQuery]String id, [FromQuery]String status)
        {
            try
            {
                if (!await IdentityFunctions.UpdateStatus(Convert.ToInt32(id), Convert.ToInt32(status)))
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
