using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/product")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class ProductController : Controller
    {
        // FRACTIONATED

        // GET services/product/FractionatedByProjectId?projectId=1
        [HttpGet("FractionatedByProjectId")]
        public async Task<ActionResult<ProductFractionated>> GetFractionatedByProjectId([FromQuery]String projectId)
        {
            try
            {
                return Ok(await ProductFractionatedFunctions.GetByProjectId(Convert.ToInt32(projectId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/product/FractionatedAllByStatus
        [HttpGet("FractionatedAllByStatus")]
        public async Task<ActionResult<IEnumerable<ProductFractionated>>> GetFractionatedAllByStatus([FromQuery]String productFractionatedStatusId)
        {
            return Ok(await ProductFractionatedFunctions.GetAllByStatus(Convert.ToInt32(productFractionatedStatusId)));
        }

        // POST services/project/RegisterFractionated
        [HttpPost("RegisterFractionated")]
        public async Task<ActionResult<int>> RegisterProductFractionated([FromBody]ProductFractionated productFractionatedRequest)
        {
            try
            {
                return Ok(await ProductFractionatedFunctions.Register(productFractionatedRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // FINANCED

        // GET services/product/FinancedByProjectId?projectId=1
        [HttpGet("FinancedByProjectId")]
        public async Task<ActionResult<ProductFinanced>> GetFinancedByProjectId([FromQuery]String projectId)
        {
            try
            {
                return Ok(await ProductFinancedFunctions.GetByProjectId(Convert.ToInt32(projectId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/product/FinancedAllBySatus
        [HttpGet("FinancedAllBySatus")]
        public async Task<ActionResult<IEnumerable<ProductFinanced>>> GetFinancedAllBySatus([FromQuery]String productFinancedStatusId)
        {
            try
            {
                return Ok(await ProductFinancedFunctions.GetAllByStatus(Convert.ToInt32(productFinancedStatusId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/project/RegisterFinanced
        [HttpPost("RegisterFinanced")]
        public async Task<ActionResult<int>> RegisterFinanced([FromBody]ProductFinanced productFinanced)
        {
            try
            {
                return Ok(await ProductFinancedFunctions.Register(productFinanced));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PREPAID

        // GET services/product/PrepaidByProjectId?projectId=1
        [HttpGet("PrepaidFullByProjectId")]
        public async Task<ActionResult<ProductPrepaid>> GetPrepaidByProjectId([FromQuery]String projectId)
        {
            try
            {
                return Ok(await ProductPrepaidFunctions.GetByProjectId(Convert.ToInt32(projectId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/product/PrepaidAllByStatus
        [HttpGet("PrepaidAllByStatus")]
        public async Task<ActionResult<IEnumerable<ProductPrepaid>>> GetPrepaidAllByStatus([FromQuery]String productPrepaidStatusId)
        {
            try
            {
                return Ok(await ProductPrepaidFunctions.GetAllByStatus(Convert.ToInt32(productPrepaidStatusId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/project/RegisterPrepaid
        [HttpPost("RegisterPrepaid")]
        public async Task<ActionResult<int>> RegisterPrepaid([FromBody]ProductPrepaid productPrepaid)
        {
            try
            {
                return Ok(await ProductPrepaidFunctions.Register(productPrepaid));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}