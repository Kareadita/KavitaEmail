using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Skeleton.Controllers;

public class AboutController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet("version")]
    public ActionResult<string> GetVersion()
    {
        return Ok(Assembly.GetExecutingAssembly().GetName().Version!.ToString());
    }
}