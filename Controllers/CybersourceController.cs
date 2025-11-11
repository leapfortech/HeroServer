using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using CyberSource.Model;

namespace HeroServer.Controllers
{
    [Route("services/cybersource")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class CybersourceController : Controller
    {
        // POST services/cybersource/Params
        [HttpPost("Params")]
        public async Task<ActionResult<String>> GetParams([FromBody] String alice)
        {
            try
            {
                return Ok(await CybersourceFunctions.GetParams(alice)); //, wsLogger));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/cybersource/PaymentInstrumentsByCustomer?customerTokenId=BD64BA5CE47BEA58E05341588E0A6960
        //[HttpGet("PaymentInstrumentsByCustomer")]
        //public async Task<ActionResult<List<String>>> GetPaymentInstrumentsByCustomer([FromQuery]String customerTokenId)
        //{
        //    try
        //    {
        //        List<Tmsv2customersEmbeddedDefaultPaymentInstrument> response = await CybersourceFunctions.GetPaymentInstrumentsByCustomer(customerTokenId);
        //        List<String> paymentInstrumentIds = new List<string>(response.Count);
        //        for (int i = 0; i < response.Count; i++)
        //            paymentInstrumentIds.Add(response[i].Id + " [" + response[i].InstrumentIdentifier.Id + "]");
        //        return Ok(paymentInstrumentIds);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // GET services/cybersource/PaymentInstrumentsByIdentifier?instrumentIdentifierId=7010000000007771639
        //[HttpGet("PaymentInstrumentsByIdentifier")]
        //public async Task<ActionResult<List<String>>> GetPaymentInstrumentsByIdentifier([FromQuery]String instrumentIdentifierId)
        //{
        //    try
        //    {
        //        List<Tmsv2customersEmbeddedDefaultPaymentInstrument> response = await CybersourceFunctions.GetPaymentInstrumentsByIdentifier(instrumentIdentifierId);
        //        List<String> paymentInstrumentIds = new List<string>(response.Count);
        //        for (int i = 0; i < response.Count; i++)
        //            paymentInstrumentIds.Add(response[i].Id);
        //        return Ok(paymentInstrumentIds);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // GET services/cybersource/GetCustomer?csToken=BDF2102417CC3449E05341588E0AFB53
        [HttpGet("GetCustomer")]
        public async Task<ActionResult<Tmsv2customersBuyerInformation>> GetCustomer([FromQuery]String csToken)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysAdmin(HttpContext, "Cybersource.GetCustomer"))
                    return Unauthorized();

                TmsV2CustomersResponse response = await CybersourceFunctions.GetCustomer(csToken);
                return Ok(response.BuyerInformation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/cybersource/GetTransaction?id=6179161085166341804004
        [HttpGet("GetTransaction")]
        public async Task<ActionResult<TransactionResponse>> GetTransaction([FromQuery]String id)
        {
            try
            {
                if (!await FirebaseFunctions.AuthorizeWebSysAdmin(HttpContext, "Cybersource.GetTransaction"))
                    return Unauthorized();

                return Ok(await CybersourceFunctions.GetTransaction(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/cybersource/RegisterCustomer?customerId=1&customerEmail=juan.perez@gmail.com
        //[HttpPost("RegisterCustomer")]
        //public async Task<ActionResult<String>> RegisterCustomer([FromQuery]String customerId, [FromQuery]String customerEmail)
        //{
        //    try
        //    {
        //        TmsV2CustomersResponse response = await CybersourceFunctions.RegisterCustomer(WebEnvConfig.Flag + customerId, customerEmail);
        //        return Ok(response.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // POST services/cybersource/RegisterInstrumentIdentifier?cardNumber=4784521596587410
        //[HttpPost("RegisterInstrumentIdentifier")]
        //public async Task<ActionResult<String>> RegisterInstrumentIdentifier([FromQuery]String cardNumber)
        //{
        //    try
        //    {
        //        Tmsv2customersEmbeddedDefaultPaymentInstrumentEmbeddedInstrumentIdentifier response = await CybersourceFunctions.RegisterInstrumentIdentifier(cardNumber);
        //        return Ok(response.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // POST services/cybersource/VoidTransaction?id=6179161085166341804004&ref=V00010300016520210408150754
        //[HttpPost("VoidTransaction")]
        //public async Task<ActionResult<VoidResponse>> VoidTransaction([FromQuery]String id, [FromQuery]String @ref)
        //{
        //    try
        //    {
        //        return Ok(await CybersourceFunctions.VoidTransaction(id, @ref));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // DELETE services/cybersource/PaymentInstrument?paymentInstrumentTokenId=BBE618914B101B50E05341588E0A3716
        //[HttpDelete("PaymentInstrument")]
        //public async Task<ActionResult<List<String>>> DelPaymentInstrument([FromQuery]String paymentInstrumentTokenId)
        //{
        //    try
        //    {
        //        await CybersourceFunctions.DelPaymentInstrument(paymentInstrumentTokenId);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // DELETE services/cybersource/AllPaymentInstrumentsByIdentifier?instrumentIdentifierTokenId=7010000000007004098
        //[HttpDelete("AllPaymentInstrumentsByIdentifier")]
        //public async Task<ActionResult<List<String>>> DelPaymentInstrumentsByIdentifier([FromQuery]String instrumentIdentifierTokenId)
        //{
        //    try
        //    {
        //        List<Tmsv2customersEmbeddedDefaultPaymentInstrument> response = await CybersourceFunctions.DelPaymentInstrumentsByIdentifier(instrumentIdentifierTokenId);

        //        List<String> paymentInstrumentIds = new List<string>(response.Count);
        //        for (int i = 0; i < response.Count; i++)
        //            paymentInstrumentIds.Add(response[i].Id);
        //        return Ok(paymentInstrumentIds);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // DELETE services/cybersource/AllPaymentInstrumentsByCustomerAndIdentifier?customerTokenId=BD64BA5CE47BEA58E05341588E0A6960&instrumentIdentifierTokenId=7010000000007004098
        //[HttpDelete("AllPaymentInstrumentsByCustomerAndIdentifier")]
        //public async Task<ActionResult<List<String>>> DelPaymentInstrumentsByCustomerAndIdentifier([FromQuery]String customerTokenId, [FromQuery]String instrumentIdentifierTokenId)
        //{
        //    try
        //    {
        //        List<Tmsv2customersEmbeddedDefaultPaymentInstrument> response = await CybersourceFunctions.DelPaymentInstrumentsByCustomerAndIdentifier(customerTokenId, instrumentIdentifierTokenId);

        //        List<String> paymentInstrumentIds = new List<string>(response.Count);
        //        for (int i = 0; i < response.Count; i++)
        //            paymentInstrumentIds.Add(response[i].Id);
        //        return Ok(paymentInstrumentIds);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // DELETE services/cybersource/AllPaymentInstrumentsByCustomer?customerTokenId=BD64BA5CE47BEA58E05341588E0A6960
        //[HttpDelete("AllPaymentInstrumentsByCustomer")]
        //public async Task<ActionResult<List<String>>> DelPaymentInstrumentsByCustomer([FromQuery]String customerTokenId)
        //{
        //    try
        //    {
        //        List<Tmsv2customersEmbeddedDefaultPaymentInstrument> response = await CybersourceFunctions.DelPaymentInstrumentsByCustomer(customerTokenId);

        //        List<String> paymentInstrumentIds = new List<string>(response.Count);
        //        for (int i = 0; i < response.Count; i++)
        //            paymentInstrumentIds.Add(response[i].Id);
        //        return Ok(paymentInstrumentIds);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}