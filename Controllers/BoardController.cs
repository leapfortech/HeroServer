using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/board")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class BoardController : Controller
    {

    }
}