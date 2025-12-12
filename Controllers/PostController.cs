using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/post")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class PostController : Controller
    {
        // GET services/post?id=1
        [HttpGet]
        public async Task<ActionResult<Post>> GetById([FromQuery]String id)
        {
            try
            {
                return Ok(await PostFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/TaleFullsByStatus/post?id=1
        [HttpGet("TaleFullsByStatus")]
        public async Task<ActionResult<Post>> TaleFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetTaleFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/RecipeFullsByStatus/post?id=1
        [HttpGet("RecipeFullsByStatus")]
        public async Task<ActionResult<Post>> RecipeFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetRecipeFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/TreatmentFullsByStatus/post?id=1
        [HttpGet("TreatmentFullsByStatus")]
        public async Task<ActionResult<Post>> TreatmentFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetTreatmentFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/RadioFullsByStatus/post?id=1
        [HttpGet("RadioFullsByStatus")]
        public async Task<ActionResult<Post>> RadioFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetRadioFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/ProductFullsByStatus/post?id=1
        [HttpGet("ProductFullsByStatus")]
        public async Task<ActionResult<Post>> ProductFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetProductFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/HappeningFullsByStatus/post?id=1
        [HttpGet("HappeningFullsByStatus")]
        public async Task<ActionResult<Post>> HappeningFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetHappeningFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/NewsFullsByStatus/post?id=1
        [HttpGet("NewsFullsByStatus")]
        public async Task<ActionResult<Post>> NewsFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetNewsFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/PuzzleFullsByStatus/post?id=1
        [HttpGet("PuzzleFullsByStatus")]
        public async Task<ActionResult<Post>> PuzzleFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PostFunctions.GetPuzzleFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterTale?appUserId=2
        [HttpPost("RegisterTale")]
        public async Task<ActionResult<long>> RegisterTale([FromBody] RegisterTaleRequest registerTaleRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterTale(registerTaleRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterRecipe?appUserId=2
        [HttpPost("RegisterRecipe")]
        public async Task<ActionResult<long>> RegisterRecipe([FromBody] RegisterRecipeRequest registerRecipeRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterRecipe(registerRecipeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterTreatment?appUserId=2
        [HttpPost("RegisterTreatment")]
        public async Task<ActionResult<long>> RegisterTreatment([FromBody] RegisterTreatmentRequest registerTreatmentRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterTreatment(registerTreatmentRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterRadio?appUserId=2
        [HttpPost("RegisterRadio")]
        public async Task<ActionResult<long>> RegisterRadio([FromBody] RegisterRadioRequest registerRadioRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterRadio(registerRadioRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterProduct?appUserId=2
        [HttpPost("RegisterProduct")]
        public async Task<ActionResult<long>> RegisterProduct([FromBody] RegisterProductRequest registerProductRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterProduct(registerProductRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterHappening?appUserId=2
        [HttpPost("RegisterHappening")]
        public async Task<ActionResult<long>> RegisterHappening([FromBody] RegisterHappeningRequest registerHappeningRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterHappening(registerHappeningRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterNews?appUserId=2
        [HttpPost("RegisterNews")]
        public async Task<ActionResult<long>> RegisterNews([FromBody] RegisterNewsRequest registerNewsRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterNews(registerNewsRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/post/RegisterPuzzle?appUserId=2
        [HttpPost("RegisterPuzzle")]
        public async Task<ActionResult<long>> RegisterPuzzle([FromBody] RegisterPuzzleRequest registerPuzzleRequest)
        {
            try
            {
                return Ok(await PostFunctions.RegisterPuzzle(registerPuzzleRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/post
        [HttpPut]
        public async Task<ActionResult<long>> Update([FromBody]Post post)
        {
            try
            {
                return Ok(await PostFunctions.Update(post));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}