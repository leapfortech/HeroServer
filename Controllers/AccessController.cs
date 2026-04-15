using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace HeroServer.Controllers
{
    [Route("services/access")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class AccessController : Controller
    {
        public AccessController(IWebHostEnvironment environment)
        {
            HtmlHelper.Initialize(environment.ContentRootPath);
        }

        // APP

        // GET services/access/LoginAppInfo?appUserId=2&webSysUserId=2
        [HttpGet("LoginAppInfo")]
        public async Task<ActionResult<LoginAppInfo>> GetLoginAppInfo([FromQuery]String appUserId, [FromQuery] String webSysUserId)
        {
            try
            {
                return Ok(await AccessFunctions.GetLoginAppInfo(Convert.ToInt64(appUserId), Convert.ToInt64(webSysUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/access/LoginApp
        [HttpPost("LoginApp")]
        public async Task<ActionResult<LoginAppResponse>> LoginApp([FromBody]LoginRequest loginRequest)
        {
            try
            {
                LoginAppResponse response = await AccessFunctions.LoginApp(HttpContext, loginRequest);
                if (response == null)
                    return Unauthorized();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/access/RegisterApp
        [HttpPost("RegisterApp")]
        public async Task<ActionResult<String>> RegisterApp([FromBody]RegisterAppRequest registerAppRequest)
        {
            try
            {
                return Ok(await AccessFunctions.RegisterApp(registerAppRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/access/Onboarding
        [HttpPost("Onboarding")]
        public async Task<ActionResult<OnboardingResponse>> Onboarding([FromBody] OnboardingRequest onboardingRequest)
        {
            try
            {
                return Ok(await AccessFunctions.Onboarding(onboardingRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/access/RegisterCSTokens
        //[HttpPost("RegisterCSTokens")]
        //public async Task<ActionResult<int>> RegisterCSTokens()
        //{
        //    try
        //    {
        //        return Ok(await AccessFunctions.RegisterCSTokens(bntConnString));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // BOARD

        // POST services/access/LoginBoard
        [HttpPost("LoginBoard")]
        public async Task<ActionResult<LoginBoardResponse>> LoginBoard([FromBody]LoginRequest loginRequest)
        {
            try
            {
                LoginBoardResponse response = await AccessFunctions.LoginBoard(HttpContext, loginRequest);
                if (response == null)
                    return Unauthorized();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/access/RegisterBoard
        [HttpPost("RegisterBoard")]
        public async Task<ActionResult<long>> RegisterBoard([FromBody]RegisterBoardRequest registerBoardRequest)
        {
            try
            {
                return Ok(await AccessFunctions.RegisterBoard(registerBoardRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/access/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<long>> ResetPassword([FromBody] PasswordRequest passwordRequest)
        {
            try
            {
                // UID ?
                //if (!await FirebaseFunctions.Authorize(bntConnString, HttpContext, eMail))
                //    return Unauthorized();

                return Ok(await AccessFunctions.ResetPassword(passwordRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/access/UpdatePassword
        [HttpPut("UpdatePassword")]
        public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            try
            {
                // UID ?
                //if (!await FirebaseFunctions.Authorize(bntConnString, HttpContext, eMail))
                //    return Unauthorized();

                await AccessFunctions.UpdatePassword(updatePasswordRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/access/ResetAccount
        [HttpPost("ResetAccount")]
        public async Task<ActionResult> ResetAccount([FromBody] AccountRequest accountRequest)
        {
            try
            {
                // UID ?
                //if (!await FirebaseFunctions.Authorize(bntConnString, HttpContext, eMail))
                //    return Unauthorized();

                await AccessFunctions.ResetAccount(accountRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/access/UpdateAccount
        [HttpPut("UpdateAccount")]
        public async Task<ActionResult> UpdateAccount([FromBody] UpdateAccountRequest updateAccountRequest)
        {
            try
            {
                // UID ?
                //if (!await FirebaseFunctions.Authorize(bntConnString, HttpContext, eMail))
                //    return Unauthorized();

                await AccessFunctions.UpdateAccount(updateAccountRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/access/id
        //[HttpDelete("{id}")]
        //public ActionResult Delete(int webSysUSerId)
        //{
        //    if (!new WebSysUserDB(bntConnString).DeleteById(webSysUSerId))
        //        return BadRequest();

        //    return Ok();
        //}
    }
}