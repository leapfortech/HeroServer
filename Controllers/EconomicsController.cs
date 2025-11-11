using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/economics")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class EconomicsController : Controller
    {
        // GET services/economics/ByInvestmentId?investmentId=1
        [HttpGet("ByInvestmentId")]
        public async Task<ActionResult<Economics>> GetByInvestmentId([FromQuery]String investmentId)
        {
            return Ok(await EconomicsFunctions.GetByInvestmentId(Convert.ToInt32(investmentId)));
        }

        // GET services/economics/IncomeByInvestmentId?investmentId=1
        [HttpGet("IncomeByInvestmentId")]
        public async Task<ActionResult<IEnumerable<Income>>> GetIncomeByInvestmentId([FromQuery]String investmentId)
        {
            return Ok(await IncomeFunctions.GetByInvestmentId(Convert.ToInt32(investmentId)));
        }

        // GET services/economics/InfoByInvestmentId?investmentId=1
        [HttpGet("InfoByInvestmentId")]
        public async Task<ActionResult<EconomicsInfo>> GetInfoByInvestmentId([FromQuery]String investmentId)
        {
            return Ok(await EconomicsFunctions.GetInfoByInvestmentId(Convert.ToInt32(investmentId)));
        }

        // POST services/economics/Register
        [HttpPost("Register")]
        public async Task<ActionResult<IEnumerable<int>>> Register([FromBody]EconomicsInfo economicsInfo)
        {
            try
            {
                return Ok(await EconomicsFunctions.Register(economicsInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/economics/Income
        [HttpPost("Income")]
        public async Task<ActionResult<int>> AddIncome([FromBody]Income income)
        {
            try
            {
                return Ok(await IncomeFunctions.Add(income));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/economics/Update
        [HttpPut("Update")]
        public async Task<ActionResult<List<int>>> Update([FromBody]EconomicsInfo economicsInfo)
        {
            try
            {
                return Ok(await EconomicsFunctions.Update(economicsInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/economics/Income
        [HttpPut("Income")]
        public async Task<ActionResult<bool>> UpdateIncome([FromBody]Income income)
        {
            try
            {
                return Ok(await IncomeFunctions.Update(income));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}