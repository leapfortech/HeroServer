using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Twilio.Rest.Verify.V2.Service;

namespace HeroServer.Controllers
{
    [Route("services/phone")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class PhoneController : Controller
    {
        // POST services/phone/SendOTP?phoneNumber=50263547362
        [HttpPost("SendOTP")]
        public async Task<ActionResult<VerificationResource>> SendOTP([FromQuery]String phoneNumber)
        {
            try
            {
                return Ok(await PhoneFunctions.SendOTP(phoneNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/phone/VerifyOTP?phoneNumber=50263547362&code=765467
        [HttpPost("VerifyOTP")]
        public async Task<ActionResult<VerificationCheckResource>> VerifyOTP([FromQuery]String phoneNumber, [FromQuery]String code)
        {
            try
            {
                return Ok(await PhoneFunctions.VerifyOTP(phoneNumber, code));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/phone/RegisterPhone?phoneCountryId=1&phoneNumber=
        [HttpPost("RegisterPhone")]
        public async Task<ActionResult<String>> RegisterPhone([FromQuery]long phoneCountryId, [FromQuery]String phoneNumber)
        {
            try
            {
                return Ok(await PhoneFunctions.RegisterPhone(phoneCountryId, phoneNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/phone/ValidateCode
        [HttpPost("ValidateCode")]
        public async Task<ActionResult<String>> ValidateCode([FromBody]PhoneCodeRequest phoneCodeRequest)
        {
            try
            {
                return Ok(await PhoneFunctions.ValidateCode(phoneCodeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/phone/PhoneInfo
        [HttpGet("PhoneInfo")]
        public async Task<ActionResult<PhoneInfo>> PhoneInfo([FromQuery]String phoneNumber, [FromQuery]bool carrier, [FromQuery]bool callerName)
        {
            try
            {
                return Ok(await PhoneFunctions.GetPhoneInfo(phoneNumber, carrier, callerName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/phone/PhoneInfoWeb
        [HttpGet("PhoneInfoWeb")]
        public async Task<ActionResult<PhoneInfo>> PhoneInfoWeb([FromQuery]String phoneNumber, [FromQuery]bool carrier, [FromQuery]bool callerName)
        {
            try
            {
                return Ok(await PhoneFunctions.GetPhoneInfoWeb(phoneNumber, carrier, callerName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}