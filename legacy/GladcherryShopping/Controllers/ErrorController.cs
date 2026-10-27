using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View("~/Views/Error/NotFound.cshtml");
        }
    }
}