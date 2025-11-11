using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using CyberSource.Model;

namespace HeroServer.Controllers
{
    [Route("services/card")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class CardController(ILogger<CardController> logger) : Controller
    {
        readonly ILogger<CardController> wsLogger = logger;

        // GET services/card
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Card>>> GetAll()
        //{
        //    return Ok(await new CardDB(bntConnString).GetAll());
        //}

        // GET services/card/ById?id=1
        //[HttpGet("ById")]
        //public async Task<ActionResult<Card>> GetById([FromQuery]String id)
        //{
        //    Card card = await new CardDB(bntConnString).GetById(Convert.ToInt32(id));

        //    if (!await FirebaseFunctions.Authorize(bntConnString, HttpContext, Convert.ToInt32(card.AppUserId), "Card.ById"))
        //        return Unauthorized();

        //    return Ok(card);
        //}

        // GET services/card/ByAppUserId?appUserId=1
        [HttpGet("ByAppUserId")]
        public async Task<ActionResult<List<Card>>> GetByAppUserId([FromQuery]String appUserId)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeAppUser(HttpContext, Convert.ToInt32(appUserId), "Card.ByAppUserId"))
                    return Unauthorized();

                return Ok(await CardFunctions.GetByAppUserId(Convert.ToInt32(appUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/card/BillTo?appUserId=1
        [HttpGet("BillTo")]
        public async Task<ActionResult<Tmsv2customersEmbeddedDefaultPaymentInstrumentBillTo>> GetBillTo([FromQuery]String appUserId)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeAppUser(HttpContext, Convert.ToInt32(appUserId), "Card.BillTo"))
                    return Unauthorized();

                return Ok(await CardFunctions.GetBillTo(Convert.ToInt32(appUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/card
        [HttpPost]
        public async Task<ActionResult<Card>> Register([FromBody]CardRegister cardRegister)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeAppUser(HttpContext, Convert.ToInt32(cardRegister.AppUserId), "Card.Register"))
                    return Unauthorized();

                return Ok(await CardFunctions.Register(cardRegister, wsLogger));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/card
        //[HttpPut]
        //public async Task<ActionResult> Update([FromBody]Card card)
        //{
        //    try
        //    {
        //        if (!await FirebaseFunctions.Authorize(bntConnString, HttpContext, Convert.ToInt32(card.AppUserId), "Card.Update"))
        //            return Unauthorized();

        //        if (!await new CardDB(bntConnString).Update(card))
        //            return BadRequest("No es posible Actualizar la Tarjeta");
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // PUT services/card/SetStatus?id=1&status=0
        [HttpPut("SetStatus")]
        public async Task<ActionResult> SetStatus([FromQuery]String id, [FromQuery]String status)
        {
            try
            {
                int cardId = Convert.ToInt32(id);
                Card card = await new CardDB().GetById(cardId);

                if (!await FirebaseFunctions.AuthorizeAppUser(HttpContext, Convert.ToInt32(card.AppUserId), "Card.SetStatus"))
                    return Unauthorized();

                await CardFunctions.SetStatus(cardId, Convert.ToInt32(status));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/card?id=1
        //[HttpDelete]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        await new CardDB(bntConnString).DeleteById(id);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}