using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/vision")]
    [Authorize("FirebaseAccess")]
    [ApiController]

    public class VisionController : Controller
    {
        // POST services/vision/DetectFaces
        [HttpPost("DetectFaces")]
        public async Task<ActionResult<LeapResponse>> DetectFaces([FromBody]VisionRequest visionRequest)
        {
            try
            {
                return Ok(await VisionFunctions.DetectFaces(HttpContext,visionRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/vision/DetectFacesCount
        [HttpPost("DetectFacesCount")]
        public async Task<ActionResult<LeapResponse>> DetectFacesCount([FromBody]VisionRequest visionRequest)
        {
            try
            {
                return Ok(await VisionFunctions.DetectFacesCount(HttpContext,visionRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/vision/CompareFaces
        [HttpPost("CompareFaces")]
        public async Task<ActionResult<LeapResponse>> CompareFaces([FromBody]VisionRequest visionRequest)
        {
            try
            {
                return Ok(await VisionFunctions.CompareFaces(HttpContext, visionRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/vision/ExtractDpiFront
        [HttpPost("ExtractDpiFront")]
        public async Task<ActionResult<LeapResponse>> ExtractDpiFront([FromBody]VisionRequest visionRequest)
        {
            try
            {
                return Ok(await VisionFunctions.ExtractDpiFront(HttpContext, visionRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/vision/ExtractDpiBack
        [HttpPost("ExtractDpiBack")]
        public async Task<ActionResult<LeapResponse>> ExtractDpiBack([FromBody]VisionRequest visionRequest)
        {
            try
            {
                return Ok(await VisionFunctions.ExtractDpiBack(HttpContext, visionRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/vision/Liveness3dKeys
        [HttpGet("Liveness3dKeys")]
        public async Task<ActionResult<LeapResponse>> Liveness3dKeys()
        {
            try
            {
                return Ok(await VisionFunctions.Liveness3dKeys());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/vision/Liveness3dToken
        [HttpGet("Liveness3dToken")]
        public async Task<ActionResult<LeapResponse>> Liveness3dToken()
        {
            try
            {
                return Ok(await VisionFunctions.Liveness3dToken());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/vision/Liveness3d
        [HttpPost("Liveness3d")]
        public async Task<ActionResult<LeapResponse>> Liveness3d([FromBody]VisionLiveness3dRequest liveness3dRequest)
        {
            try
            {
                return Ok(await VisionFunctions.Liveness3d(HttpContext, liveness3dRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}