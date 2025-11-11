using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/project")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class ProjectController : Controller
    {
        // GET FULL
        // GET services/project/Fulls?status=1
        [HttpGet("Fulls")]
        public async Task<ActionResult<ProjectProductFull[]>> GetFulls([FromQuery]String status = "-1")
        {
            try
            {
                return Ok(await ProjectFunctions.GetFulls(Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/project/FullsByAppUser
        [HttpGet("FullsByAppUser")]
        public async Task<ActionResult<ProjectProductFull[]>> GetFullsByAppUser([FromQuery]String appUserId)
        {
            try
            {
                return Ok(await ProjectFunctions.GetFullsByAppUser(Convert.ToInt32(appUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/project/Info?id=1&images=true
        [HttpGet("Info")]
        public async Task<ActionResult<ProjectInfo>> GetInfo([FromQuery]String id, [FromQuery]String images = "true")
        {
            try
            {
                return Ok(await ProjectFunctions.GetInfo(Convert.ToInt32(id), images == "true"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/project/LikeIdsByAppUserId?appUserId=1
        [HttpGet("LikeIdsByAppUserId")]
        public async Task<ActionResult<List<int>>> GetLikeIdsByAppUserId([FromQuery]String appUserId)
        {
            try
            {
                return Ok(await ProjectLikeFunctions.GetIdsByAppUserId(Convert.ToInt32(appUserId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/project/Images?first=false
        [HttpGet("Images")]
        public async Task<ActionResult<List<ProjectImages>>> GetImages([FromQuery]String first = "true")
        {
            try
            {
                return Ok(await ProjectFunctions.GetImages(first == "true"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/project/ImagesById?id=1&first=true
        [HttpGet("ImagesById")]
        public async Task<ActionResult<List<String>>> GetImagesById([FromQuery]String id, [FromQuery]String first = "true")
        {
            try
            {
                return Ok(await ProjectFunctions.GetImagesById(Convert.ToInt32(id), first == "true"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // REGISTER

        // POST services/project/Register
        [HttpPost("Register")]
        public async Task<ActionResult<int>> Register([FromBody]ProjectInfo projectRequest)
        {
            try
            {
                return Ok(await ProjectFunctions.Register(projectRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/project/RegisterLike
        [HttpPost("RegisterLike")]
        public async Task<ActionResult<int>> RegisterLike([FromBody]ProjectLike projectLike)
        {
            try
            {
                return Ok(await ProjectLikeFunctions.Register(projectLike));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // UPDATE

        // PUT services/project/LikeStatus?id=5&status=1
        [HttpPut("LikeStatus")]
        public async Task<ActionResult<bool>> UpdateLikeStatus([FromQuery]String id, [FromQuery]String status)
        {
            try
            {
                return Ok(await ProjectLikeFunctions.UpdateStatus(Convert.ToInt32(id), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/project/
        [HttpPut]
        public async Task<ActionResult> Update([FromBody]ProjectInfo projectInfo)
        {
            try
            {
                await ProjectFunctions.Update(projectInfo);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}