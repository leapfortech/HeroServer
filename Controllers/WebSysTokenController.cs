using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/websystoken")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class SystemUTokenController : Controller
    {
        // GET services/websystoken
        //[HttpGet]
        //public async Task<ActionResult<List<WebSysToken>>> Get()
        //{
        //    return Ok(await new WebSysTokenDB(bntConnString).GetAll());
        //}

        // GET services/websystoken/ById?id=
        //[HttpGet("ById")]
        //public async Task<ActionResult<WebSysToken>> GetById([FromQuery]String id)
        //{
        //    return Ok(await new SystemTokenDB(bntConnString).GetById(Convert.ToInt32(id)));
        //}

        // GET services/websystoken/ByWebSysUserId?webSysUserId=3
        [HttpGet("ByWebSysUserId")]
        public async Task<ActionResult<List<WebSysToken>>> GetByWebSysUserId([FromQuery] String webSysUserId)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(HttpContext, Convert.ToInt32(webSysUserId), "WebSysToken.ByWebSysUserId"))
                    return Unauthorized();

                return Ok(await new WebSysTokenDB().GetByWebSysUserId(Convert.ToInt32(webSysUserId), 1));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/websystoken/Find
        //[HttpPost("Find")]
        //public async Task<ActionResult<int>> Find([FromBody]SystemToken systemToken)
        //{
        //    return Ok(await new SystemTokenDB(bntConnString).Find(systemToken));
        //}

        // POST services/websystoken
        [HttpPost]
        public async Task<ActionResult<int>> Add([FromBody] WebSysToken webSysToken)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysUser(HttpContext, Convert.ToInt32(webSysToken.WebSysUserId), "WebSysToken.Add"))
                    return Unauthorized();

                return Ok(await WebSysTokenFunctions.FindAdd(webSysToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/websystoken
        //[HttpPut]
        //public async Task<ActionResult> Update([FromBody]SystemToken webSysToken)
        //{
        //    if (!await new SystemTokenDB(bntConnString).Update(webSysToken))
        //        return BadRequest();

        //    return Ok();
        //}

        // DELETE services/websystoken/id
        //[HttpDelete("{id}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    if (!await new SystemTokenDB(bntConnString).DeleteById(id))
        //        return BadRequest();

        //    return Ok();
        //}
    }
}