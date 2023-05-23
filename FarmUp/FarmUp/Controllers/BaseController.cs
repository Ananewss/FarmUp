using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FarmUp.Controllers
{
    public class BaseController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IConfiguration _config;

        private readonly ILogger<BaseController> _logger;

        public BaseController(IWebHostEnvironment webHostEnvironment, IConfiguration config, ILogger<BaseController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext?.RouteData?.Values["Controller"]?.ToString() ?? "";
            string actionName = filterContext?.RouteData.Values["Action"]?.ToString() ?? "";
            var lineUserID = HttpContext.Session.GetString("lineUserId");
            if (controllerName.ToLower() == "seller" && (actionName.ToLower() != "index" && actionName.ToLower() != "getliff"))
            {
                HttpContext.Session.SetString("controllerRedirect", controllerName);
                HttpContext.Session.SetString("actionRedirect", actionName);
                if (!string.IsNullOrWhiteSpace(lineUserID))
                {
                    if (Request.QueryString.HasValue)
                    {
                        var refstr = Request.Query["ref"];
                        if (!String.IsNullOrEmpty(refstr))
                        {
                            var r = refstr.ToString().Split('-');
                            filterContext.Result = RedirectToAction(r[1], r[0]);
                            return;
                        }
                    }
                }
                else
                {
                    filterContext.Result = RedirectToAction("Index", "Seller");
                    return;
                }
            }
           
        }

    }
}
