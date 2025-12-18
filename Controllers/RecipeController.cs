using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/recipe")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class RecipeController : Controller
    {
        // GET services/recipe?id=1
        [HttpGet]
        public async Task<ActionResult<Recipe>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await RecipeFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/recipe/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<RecipeFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await RecipeFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/recipe/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterRecipeRequest registerRecipeRequest)
        {
            try
            {
                return Ok(await RecipeFunctions.Register(registerRecipeRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/recipe
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] Recipe recipe)
        {
            try
            {
                return Ok(await RecipeFunctions.Update(recipe));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}