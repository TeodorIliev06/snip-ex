namespace SnipEx.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            Response.StatusCode = statusCode;

            return statusCode switch
            {
                403 => View("403"),
                404 => View("404"),
                500 => View("500"),
                _ => View("500")
            };
        }
    }
}