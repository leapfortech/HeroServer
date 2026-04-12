using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Twilio.Rest.Verify.V2.Service;

namespace HeroServer.Controllers
{
    [Route("services/precheck")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class PrecheckController : Controller
    {
        // SMS
        
        // POST services/precheck/SendOTPSms?phoneNumber=50263547362
        [HttpPost("SendOTPSms")]
        public async Task<ActionResult<VerificationResource>> SendOTPSms([FromQuery]String phoneNumber)
        {
            try
            {
                return Ok(await PrecheckFunctions.SendOTPSms(phoneNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/precheck/VerifyOTPSms?phoneNumber=50263547362&code=765467
        [HttpPost("VerifyOTPSms")]
        public async Task<ActionResult<VerificationCheckResource>> VerifyOTPSms([FromQuery]String phoneNumber, [FromQuery]String code)
        {
            try
            {
                return Ok(await PrecheckFunctions.VerifyOTPSms(phoneNumber, code));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/precheck/RegisterPhoneSms?phoneCountryId=1&phoneNumber=
        [HttpPost("RegisterPhoneSms")]
        public async Task<ActionResult<String>> RegisterPhoneSms([FromQuery]long phoneCountryId, [FromQuery]String phoneNumber)
        {
            try
            {
                return Ok(await PrecheckFunctions.RegisterPhoneSms(phoneCountryId, phoneNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/precheck/ValidatePhoneSmsCode
        [HttpPost("ValidatePhoneSmsCode")]
        public async Task<ActionResult<String>> ValidatePhoneSmsCode([FromBody]PhoneCodeRequest phoneCodeRequest)
        {
            try
            {
                return Ok(await PrecheckFunctions.ValidatePhoneSmsCode(phoneCodeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/precheck/GetPhoneInfoTwilio
        [HttpGet("GetPhoneInfoTwilio")]
        public async Task<ActionResult<PhoneInfo>> GetPhoneInfoTwilio([FromQuery]String phoneNumber, [FromQuery]bool carrier, [FromQuery]bool callerName)
        {
            try
            {
                return Ok(await PrecheckFunctions.GetPhoneInfoTwilio(phoneNumber, carrier, callerName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/precheck/GetPhoneInfoWebTwilio
        [HttpGet("GetPhoneInfoWebTwilio")]
        public async Task<ActionResult<PhoneInfo>> GetPhoneInfoWebTwilio([FromQuery]String phoneNumber, [FromQuery]bool carrier, [FromQuery]bool callerName)
        {
            try
            {
                return Ok(await PrecheckFunctions.GetPhoneInfoWebTwilio(phoneNumber, carrier, callerName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // WHATSAPP
        // POST services/precheck/RegisterPhoneWA?phoneCountryId=1&phoneNumber=
        [HttpPost("RegisterPhoneWA")]
        public async Task<ActionResult<String>> RegisterPhoneWA([FromQuery] long phoneCountryId, [FromQuery] String phoneNumber)
        {
            try
            {
                return Ok(await PrecheckFunctions.RegisterPhoneWA(phoneCountryId, phoneNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/precheck/ValidatePhoneWACode
        [HttpPost("ValidatePhoneWACode")]
        public async Task<ActionResult<String>> ValidatePhoneWACode([FromBody] PhoneCodeRequest phoneCodeRequest)
        {
            try
            {
                return Ok(await PrecheckFunctions.ValidatePhoneWACode(phoneCodeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EMAIL
        // POST services/precheck/RegisterEmail?email=
        [HttpPost("RegisterEmail")]
        public async Task<ActionResult<String>> RegisterEmail([FromQuery] String email)
        {
            try
            {
                return Ok(await PrecheckFunctions.RegisterEmail(email));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/precheck/ValidateEmailCode
        [HttpPost("ValidateEmailCode")]
        public async Task<ActionResult<String>> ValidateEmailCode([FromBody] EmailCodeRequest emailCodeRequest)
        {
            try
            {
                return Ok(await PrecheckFunctions.ValidateEmailCode(emailCodeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}