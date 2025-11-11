using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace HeroServer.Controllers
{
    [Route("services/document")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class DocumentController(IWebHostEnvironment environment) : Controller
    {
        readonly String path = environment.ContentRootPath;

        // POST services/document
        [HttpPost]
        public async Task<ActionResult<String>> CreateAgreement([FromQuery]String eMail)
        {
            return Ok(await DocumentFunctions.CreateAgreement(path, eMail));
        }
    }
}
