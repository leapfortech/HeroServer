using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace HeroServer.Controllers
{
    [Route("services/websysuser")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class WebSysUserController : Controller
    {
        public WebSysUserController(IWebHostEnvironment environment)
        {
            HtmlHelper.Initialize(environment.ContentRootPath);
        }

        // GET services/websysuser
        [HttpGet]
        public async Task<ActionResult<List<WebSysUser>>> Get()
        {
            return Ok(await new WebSysUserDB().GetAll());
        }

        // GET services/websysuser/ById?id=
        //[HttpGet("ById")]
        //public async Task<ActionResult<WebSysUser>> GetById([FromQuery]String id)
        //{
        //    return Ok(await new WebSysUserDB().GetById(Convert.ToInt32(id)));
        //}

        // POST services/websysuser
        //[HttpPost]
        //public async Task<ActionResult<int>> Post([FromBody]WebSysUser webSysUser)
        //{
        //    try
        //    {
        //        return Ok(await new WebSysUserDB().Add(webSysUser));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //-------------------------------------------------------------------------------------------------
        // ROLES

        // PUT services/websysuser/UpdateRoles?id=3&roles=BS|BOI
        [HttpPut("UpdateRoles")]
        public async Task<ActionResult> UpdateRoles([FromQuery]String id, [FromQuery]String roles)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysAdmin(HttpContext, "WebSysUser.UpdateRoles"))
                    return Unauthorized();

                await WebSysUserFunctions.UpdateRoles(Convert.ToInt64(id), roles);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //-------------------------------------------------------------------------------------------------
        // EMAIL

        // GET services/websysuser/SendMailLink?eMail=name@gmail.com
        [HttpGet("SendMailLink")]
        public async Task<ActionResult> SendMailLink([FromQuery] String eMail)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeMail(HttpContext, eMail, "WebSysUser.VerifyMail"))
                    return Unauthorized();

                await WebSysUserFunctions.SendMailLink(eMail);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/websysuser/ConfirmMail?token=HFJ7834
        [HttpGet("ConfirmMail")]
        [AllowAnonymous]
        public async Task<ContentResult> ConfirmMail([FromQuery] String token)
        {
            return await WebSysUserFunctions.ConfirmMail(token);
        }

        // GET services/websysuser/Encrypt?text=HFJ7834
        //[HttpGet("Encrypt")]
        //[AllowAnonymous]
        //public String Encrypt([FromQuery] String text)
        //{
        //    return WebSysUserFunctions.Encrypt(text);
        //}

        // GET services/websysuser/Decrypt?token=HFJ7834
        //[HttpGet("Decrypt")]
        //[AllowAnonymous]
        //public String Decrypt([FromQuery] String token)
        //{
        //    return WebSysUserFunctions.Decrypt(token);
        //}

        // GET services/websysuser/ConfirmAuthUser?token=HFJ7834
        //[HttpGet("ConfirmAuthUser")]
        //[AllowAnonymous]
        //public async Task<ContentResult> ConfirmAuthUser([FromQuery] String authUserId)
        //{
        //    return await WebSysUserFunctions.ConfirmAuthUser(authUserId);
        //}

        // PUT services/websysuser/UpdateMail?webSysUserId=53&eMail=user@gmail.com
        //[HttpPut("UpdateMail")]
        //public async Task<ActionResult> UpdateMail([FromQuery]String webSysUserId, [FromQuery]String eMail)
        //{
        //    try
        //    {
        //        if (!await FirebaseFunctions.AuthorizeWebSysUser(bntConnString, HttpContext, Convert.ToInt32(webSysUserId), "WebSysUser.UpdateMail"))
        //            return Unauthorized();

        //        await WebSysUserFunctions.UpdateMail(bntConnString, Convert.ToInt32(webSysUserId), eMail);
        //        return Ok("EMail Changed to " + eMail);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //-------------------------------------------------------------------------------------------------
        // PHONE

        // PUT services/websysuser/UpdatePhone
        [HttpPut("UpdatePhone")]
        public async Task<ActionResult> UpdatePhone([FromBody]PhoneRequest phoneRequest)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(HttpContext, phoneRequest.Id, "WebSysUser.UpdatePhone"))
                    return Unauthorized();

                await WebSysUserFunctions.UpdatePhone(phoneRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //-------------------------------------------------------------------------------------------------
        // PASSWORD

        // PUT services/websysuser/SetPassword
        [HttpPut("SetPassword")]
        public async Task<ActionResult<int>> SetPassword([FromBody]WebSysPasswordRequest passwordRequest)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(HttpContext, Convert.ToInt64(passwordRequest.WebSysUserId), "WebSysUser.SetPassword"))
                    return Unauthorized();

                return Ok(await WebSysUserFunctions.SetPassword(passwordRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/websysuser/ResetPassword?eMail=name@gmail.com
        [HttpGet("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromQuery]String eMail)
        {
            try
            {
                // UID ?
                //if (!await FirebaseFunctions.Authorize(bntConnString, HttpContext, eMail))
                //    return Unauthorized();

                await WebSysUserFunctions.ResetPassword(eMail);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/websysuser/EnterPassword?token=HFJ7834
        [HttpGet("EnterPassword")]
        [AllowAnonymous]
        public ContentResult EnterPassword([FromQuery]String token)
        {
            return WebSysUserFunctions.EnterPassword(token);
        }

        // POST services/websysuser/ChangePassword?token=HFJ7834
        [HttpPost("ChangePassword")]
        [AllowAnonymous]
        public async Task<ContentResult> ChangePassword([FromQuery] String token)
        {
            try
            {
                String body = String.Empty;
                using (StreamReader streamReader = new StreamReader(Request.Body))
                {
                    body = await streamReader.ReadToEndAsync();
                }
                return await WebSysUserFunctions.ChangePassword(token, body);
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", ex.Message, "#F00000")
                };
            }
        }
        /*
        //-------------------------------------------------------------------------------------------------
        // PIN

        // POST services/websysuser/VerifyPin
        [HttpPost("VerifyPin")]
        public async Task<ActionResult<String>> VerifyPin([FromBody] WebSysPinRequest pinRequest)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(bntConnString, HttpContext, pinRequest.WebSysUserId, "WebSysUser.VerifyPin"))
                    return Unauthorized();

                String res = await WebSysUserFunctions.VerifyPin(bntConnString, pinRequest);
                if (res == null)
                    return BadRequest();

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/websysuser/SetPin
        [HttpPut("SetPin")]
        public async Task<ActionResult> SetPin([FromBody] WebSysPinRequest pinRequest)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(bntConnString, HttpContext, pinRequest.WebSysUserId, "WebSysUser.SetPin"))
                    return Unauthorized();

                if (!await WebSysUserFunctions.SetPin(bntConnString, pinRequest))
                    return BadRequest();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/websysuser/ResetPin?id=1
        [HttpPut("ResetPin")]
        public async Task<ActionResult> ResetPin([FromQuery] String id)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(bntConnString, HttpContext, Convert.ToInt64(id), "WebSysUser.ResetPin"))
                    return Unauthorized();

                await WebSysUserFunctions.ResetPin(bntConnString, Convert.ToInt64(id));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/websysuser/EnterPin?token=HFJ7834
        [HttpGet("EnterPin")]
        [AllowAnonymous]
        public ContentResult EnterPin([FromQuery] String token)
        {
            return WebSysUserFunctions.EnterPin(token);
        }

        // POST services/websysuser/ChangePin?token=HFJ7834
        [HttpPost("ChangePin")]
        [AllowAnonymous]
        public async Task<ContentResult> ChangePin([FromQuery] String token)
        {
            try
            {
                String body = String.Empty;
                using (StreamReader streamReader = new StreamReader(Request.Body))
                {
                    body = await streamReader.ReadToEndAsync();
                }
                return await WebSysUserFunctions.ChangePin(bntConnString, token, body);
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = HtmlHelper.GetConfirmResultHtml("ERROR", ex.Message, "#F00000")
                };
            }
        }
        */

        // PUT services/websysuser
        //[HttpPut]
        //public async Task<ActionResult> Put([FromBody] WebSysUser webSysUser)
        //{
        //    if (!await new WebSysUserDB(bntConnString).Update(webSysUser))
        //        return BadRequest();

        //    return Ok();
        //}

        // DELETE services/websysuser/id
        //[HttpDelete("{id}")]
        //public async Task<ActionResult> Delete(long id)
        //{
        //    if (!await new WebSysUserDB(bntConnString).DeleteById(id))
        //        return BadRequest();

        //    return Ok();
        //}
    }
}