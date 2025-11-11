using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/storage")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class StorageController : Controller
    {
        // CONTAINER

        // POST services/storage/Container?name=container01
        [HttpPost("Container")]
        public async Task<ActionResult> CreateContainer([FromQuery]String name)
        {
            try
            {
                await StorageFunctions.CreateContainer(name);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/storage/Container?name=container01
        [HttpDelete("Container")]
        public async Task<ActionResult> DeleteContainer([FromQuery]String name)
        {
            try
            {
                await StorageFunctions.DeleteContainer(name);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // FILE

        // GET services/storage/File?containerName=container01&fileName=file01
        [HttpGet("File")]
        public async Task<ActionResult<String>> ReadFile([FromQuery] String containerName, [FromQuery] String fileName, [FromQuery] String fileExt)
        {
            try
            {
                return Ok(Convert.ToBase64String(await StorageFunctions.ReadFile(containerName, fileName, fileExt)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/storage/File
        [HttpPost("File")]
        public async Task<ActionResult> CreateFile([FromBody]StorageInfo storageInfo)
        {
            try
            {
                await StorageFunctions.CreateFile(storageInfo.ContainerName, storageInfo.FileName, storageInfo.FileExt, Convert.FromBase64String(storageInfo.Content));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/storage/File?name=container01
        [HttpPut("File")]
        public async Task<ActionResult> UpdateFile([FromBody]StorageInfo storageInfo)
        {
            try
            {
                await StorageFunctions.UpdateFile(storageInfo.ContainerName, storageInfo.FileName, storageInfo.FileExt, Convert.FromBase64String(storageInfo.Content), storageInfo.Backup != 0);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE services/storage/File?containerName=container01&fileName=file01.ext
        [HttpDelete("File")]
        public async Task<ActionResult> DeleteFile([FromQuery]String containerName, [FromQuery]String fileName)
        {
            try
            {
                await StorageFunctions.DeleteFile(containerName, fileName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}