using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/puzzle")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class PuzzleController : Controller
    {
        // GET services/puzzle?id=1
        [HttpGet]
        public async Task<ActionResult<Puzzle>> GetById([FromQuery] String id)
        {
            try
            {
                return Ok(await PuzzleFunctions.GetById(Convert.ToInt64(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/puzzle/FullsByStatus/?status=1
        [HttpGet("FullsByStatus")]
        public async Task<ActionResult<List<PuzzleFull>>> GetFullsByStatus([FromQuery] String status)
        {
            try
            {
                return Ok(await PuzzleFunctions.GetFullsByStatus(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/puzzle/Register
        [HttpPost("Register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterPuzzleRequest registerPuzzleRequest)
        {
            try
            {
                return Ok(await PuzzleFunctions.Register(registerPuzzleRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/puzzle
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] RegisterPuzzleRequest registerPuzzleRequest)
        {
            try
            {
                return Ok(await PuzzleFunctions.Update(registerPuzzleRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}