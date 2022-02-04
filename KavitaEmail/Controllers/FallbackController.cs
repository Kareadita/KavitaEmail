using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Skeleton.Controllers
{
    /// <summary>
    /// This is the fallback controller. Any unknown routes will flow through here and load the UI (if exists)
    /// </summary>
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }
    }
}