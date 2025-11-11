using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/investment")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class InvestmentController : Controller
    {
        // GET FULLS

        // GET services/investment/FullsByStatus?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<InvestmentResponse>> GetFullsByStatus([FromQuery]String status)
        {
            return Ok(await InvestmentFunctions.GetFullsByStatus(Convert.ToInt32(status)));
        }

        // GET services/investment/FullsByAppUserId
        [HttpGet("FullsByAppUserId")]
        public async Task<ActionResult<InvestmentResponse>> GetFullsByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await InvestmentFunctions.GetFullsByAppUserId(Convert.ToInt32(appUserId)));
        }

        // INVESTMENT FRACTIONATED

        // GET services/investment/FractionatedFullById?id=1
        [HttpGet("FractionatedFullById")]
        public async Task<ActionResult<InvestmentFractionatedFull>> GetFractionatedFullById([FromQuery]String id)
        {
            return Ok(await InvestmentFractionatedFunctions.GetFullByInvestmentId(Convert.ToInt32(id)));
        }

        // GET services/investment/FractionatedFullsByStatus?status=1
        [HttpGet("FractionatedFullsByStatus")]
        public async Task<ActionResult<IEnumerable<InvestmentFractionatedFull>>> GetFractionatedFullsByStatus([FromQuery]String status)
        {
            return Ok(await InvestmentFractionatedFunctions.GetFullsByStatus(Convert.ToInt32(status)));
        }

        // GET services/investment/FractionatedFullsByAppUserId?appUserId=1&status=-1
        [HttpGet("FractionatedFullsByAppUserId")]
        public async Task<ActionResult<IEnumerable<InvestmentFractionatedFull>>> GetFractionatedFullsByAppUserId([FromQuery]String appUserId, [FromQuery]String status)
        {
            return Ok(await InvestmentFractionatedFunctions.GetFullsByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
        }

        // INVESTMENT FINANCED

        // GET services/investment/FinancedFullById?id=1
        [HttpGet("FinancedFullById")]
        public async Task<ActionResult<InvestmentFinancedFull>> GetFinancedFullById([FromQuery]String id)
        {
            return Ok(await InvestmentFinancedFunctions.GetFullByInvestmentId(Convert.ToInt32(id)));
        }

        // GET services/investment/FinancedFullsByStatus?status=1
        [HttpGet("FinancedFullsByStatus")]
        public async Task<ActionResult<IEnumerable<InvestmentFinancedFull>>> GetFinancedFullsByStatus([FromQuery]String status)
        {
            return Ok(await InvestmentFinancedFunctions.GetFullsByStatus(Convert.ToInt32(status)));
        }

        // GET services/investment/FinancedFullsByAppUserId?appUserId=1&status=-1
        [HttpGet("FinancedFullsByAppUserId")]
        public async Task<ActionResult<IEnumerable<InvestmentFinancedFull>>> GetInvestmentFinancedFullsByAppUserId([FromQuery]String appUserId, [FromQuery]String status)
        {
            return Ok(await InvestmentFinancedFunctions.GetFullsByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
        }

        // INVESTMENT PREPAID

        // GET services/investment/PrepaidFullById?id=1
        [HttpGet("PrepaidFullById")]
        public async Task<ActionResult<InvestmentPrepaidFull>> GetPrepaidFullById([FromQuery]String id)
        {
            return Ok(await InvestmentPrepaidFunctions.GetFullByInvestmentId(Convert.ToInt32(id)));
        }

        // GET services/investment/PrepaidFullsByStatus?status=1
        [HttpGet("PrepaidFullsByStatus")]
        public async Task<ActionResult<IEnumerable<InvestmentPrepaidFull>>> GetInvestmentPrepaidFulls([FromQuery]String status)
        {
            return Ok(await InvestmentPrepaidFunctions.GetFullsByStatus(Convert.ToInt32(status)));
        }

        // GET services/investment/PrepaidFullsByAppUserId?appUserId=1&status=-1
        [HttpGet("PrepaidFullsByAppUserId")]
        public async Task<ActionResult<IEnumerable<InvestmentPrepaidFull>>> GetInvestmentPrepaidFullsByAppUserId([FromQuery]String appUserId, [FromQuery]String status)
        {
            return Ok(await InvestmentPrepaidFunctions.GetFullsByAppUserId(Convert.ToInt32(appUserId), Convert.ToInt32(status)));
        }

        // DOCS

        // GET services/investment/DocRtu?id=2
        [HttpGet("DocRtu")]
        public async Task<ActionResult<List<String>>> GetDocRtu([FromQuery]String id)
        {
            return Ok(await InvestmentFunctions.GetDocRtu(Convert.ToInt32(id)));
        }

        // GET services/investment/DocBank?id=2
        [HttpGet("DocBank")]
        public async Task<ActionResult<List<String>>> GetDocBank([FromQuery]String id)
        {
            return Ok(await InvestmentFunctions.GetDocBank(Convert.ToInt32(id)));
        }

        // GET services/investment/DocInfosByStatus?status=1
        [HttpGet("DocInfosByStatus")]
        public async Task<ActionResult<InvestmentDocInfo>> GetDocInfosByStatus([FromQuery]String status)
        {
            return Ok(await InvestmentFunctions.GetDocInfosByStatus(Convert.ToInt32(status)));
        }

        // REGISTER INVESTMENT

        // POST services/investment/RegisterFractionated
        [HttpPost("RegisterFractionated")]
        public async Task<ActionResult<InvestmentFractionatedFull>> RegisterFractionated([FromBody]Investment investment)
        {
            try
            {
                return Ok(await InvestmentFractionatedFunctions.Register(investment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/RegisterFinanced
        [HttpPost("RegisterFinanced")]
        public async Task<ActionResult<InvestmentFinancedFull>> RegisterFinanced([FromBody]Investment investment)
        {
            try
            {
                return Ok(await InvestmentFinancedFunctions.Register(investment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/RegisterPrepaid
        [HttpPost("RegisterPrepaid")]
        public async Task<ActionResult<InvestmentPrepaidFull>> RegisterPrepaid([FromBody]Investment investment)
        {
            try
            {
                return Ok(await InvestmentPrepaidFunctions.Register(investment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // REGISTER DOCS

        // POST services/investment/RegisterDocRtu
        [HttpPost("RegisterDocRtu")]
        public async Task<ActionResult> RegisterDocRtu([FromBody]InvestmentDocRequest investmentRequest)
        {
            try
            {
                await InvestmentFunctions.RegisterDocRtu(investmentRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/RegisterDocBank
        [HttpPost("RegisterDocBank")]
        public async Task<ActionResult> RegisterDocBank([FromBody]InvestmentDocRequest investmentRequest)
        {
            try
            {
                await InvestmentFunctions.RegisterDocBank(investmentRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // REGISTER REFERENCE

        // POST services/investment/RegisterReference
        [HttpPost("RegisterReference")]
        public async Task<ActionResult<List<int>>> RegisterReference([FromBody]InvestmentReference[] investmentReferences)
        {
            try
            {
                return Ok(await InvestmentFunctions.RegisterReference(investmentReferences));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/RegisterSignatory
        [HttpPost("RegisterSignatory")]
        public async Task<ActionResult<List<int>>> RegisterSignatory([FromBody]InvestmentIdentityRequest[] investmentIdentityRequests)
        {
            try
            {
                return Ok(await InvestmentFunctions.RegisterSignatory(investmentIdentityRequests));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/CreateSignatory
        [HttpPost("CreateSignatory")]
        public async Task<ActionResult> CreateSignatory([FromQuery]int investmentId)
        {
            try
            {
                await InvestmentFunctions.CreateSignatory(investmentId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/RegisterBeneficiary
        [HttpPost("RegisterBeneficiary")]
        public async Task<ActionResult<List<int>>> RegisterBeneficiary([FromBody]InvestmentIdentityRequest[] investmentIdentityRequests)
        {
            try
            {
                return Ok(await InvestmentFunctions.RegisterBeneficiary(investmentIdentityRequests));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/CreateBeneficiary
        [HttpPost("CreateBeneficiary")]
        public async Task<ActionResult> CreateBeneficiary([FromQuery]int investmentId)
        {
            try
            {
                await InvestmentFunctions.CreateBeneficiary(investmentId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // VALIDATION

        // PUT services/investment/RequestUpdate
        [HttpPut("RequestUpdate")]
        public async Task<ActionResult> RequestUpdate([FromBody]InvestmentBoardResponse boardResponse)
        {
            try
            {
                await InvestmentFunctions.RequestUpdate(boardResponse);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/investment/Authorize
        [HttpPut("Authorize")]
        public async Task<ActionResult> Authorize([FromBody]InvestmentBoardResponse boardResponse)
        {
            try
            {
                await InvestmentFunctions.Authorize(boardResponse);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/investment/Reject
        [HttpPut("Reject")]
        public async Task<ActionResult> Reject([FromBody]InvestmentBoardResponse boardResponse)
        {
            try
            {
                await InvestmentFunctions.Reject(boardResponse);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // UPDATE

        // PUT services/investment/UpdateDocRtu
        [HttpPut("UpdateDocRtu")]
        public async Task<ActionResult> UpdateDocRtu([FromBody]InvestmentDocRequest investmentRequest)
        {
            try
            {
                await InvestmentFunctions.UpdateDocRtu(investmentRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/investment/UpdateDocBank
        [HttpPut("UpdateDocBank")]
        public async Task<ActionResult> UpdateDocBank([FromBody]InvestmentDocRequest investmentRequest)
        {
            try
            {
                await InvestmentFunctions.UpdateDocBank(investmentRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PAYMENT

        // GET services/investment/BankPaymentFullsByStatus?status=2
        [HttpGet("BankPaymentFullsByStatus")]
        public async Task<ActionResult<List<InvestmentPaymentBankFull>>> GetBankPaymentFullsByStatus([FromQuery]String status)
        {
            try
            {
                return Ok(await InvestmentPaymentFunctions.GetBankFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/PaymentBank
        [HttpPost("PaymentBank")]
        public async Task<ActionResult<InvestmentBankPayment>> PaymentBank([FromBody]InvestmentBankPayment investmentBankPayment)
        {
            try
            {
                return Ok(await InvestmentPaymentFunctions.PaymentBank(investmentBankPayment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/PaymentCard
        [HttpPost("PaymentCard")]
        public async Task<ActionResult<InvestmentCardPayment>> PaymentCard([FromBody]InvestmentCardPayment investmentCardPayment)
        {
            try
            {
                return Ok(await InvestmentPaymentFunctions.PaymentCard(investmentCardPayment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/PaymentAuthorize?repaymentId=1
        [HttpPost("PaymentAuthorize")]
        public async Task<ActionResult> PaymentAuthorize([FromQuery]String boardUserId, [FromQuery]String paymentId)
        {
            try
            {
                await InvestmentPaymentFunctions.Authorize(Convert.ToInt32(boardUserId), Convert.ToInt32(paymentId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/PaymentReject?boardUserId=4&repaymentId=1&receipt=1
        [HttpPost("PaymentReject")]
        public async Task<ActionResult> PaymentReject([FromQuery]String boardUserId, [FromQuery]String paymentId, [FromQuery]String receipt)
        {
            try
            {
                await InvestmentPaymentFunctions.Reject(Convert.ToInt32(boardUserId), Convert.ToInt32(paymentId), receipt == "1");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/investment/PaymentAcknowledge?investmentPaymentId=43
        //[HttpPost("PaymentAcknowledge")]
        //public async Task<ActionResult<InvestmentPayment>> PaymentAcknowledge([FromQuery]int investmentPaymentId)
        //{
        //    try
        //    {
        //        return Ok(await InvestmentPaymentFunctions.Acknowledge(investmentPaymentId));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}