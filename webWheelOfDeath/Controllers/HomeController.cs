using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using webWheelOfDeath.Models;

namespace webWheelOfDeath.Controllers
{
    // controllers are containers for actions.
    // every public method in a controller class is an action.

    // controller doesn't HAVE to inherit from Controller.

    // temporary, doesnt survive round trips
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        // ##################### GENERAL ACTIONS ##################### \\
        // every controller action returns a result (ContentResult)
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // how to pass data to controller actions: use parameters

        // routing
        // when the user enters a url named [controller]/post/[value], [value] will be returned as a contentresult
        // so, when navigating to any controller in a url, actions within that controller can
        // be navigated to within that... and they will be run!!
        public IActionResult Post(string id)
        {
            // contentResult:
            // the expected return value from an action
            return new ContentResult { Content = id };
        }

        public IActionResult Index()
        {
            // define all controller logic when it's time to process requests
            return View();
        }

        // ##################### LOGIN ACTIONS ##################### \\
        [HttpPost]
        [Route("Login")]
        //[Route("home/login/{userType:string}")] then add in constructor Login(string userType)
        public IActionResult Login(CCredentials credentials)
        {

            // Database API silliness

            // if login succeeds
            string sessionId = $"session-{Random.Shared.Next(1000, int.MaxValue).ToString()}";
            HttpContext.Session.SetString("session-id", sessionId);

            // CLEAR THE MODELSTATE ARRGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
            ModelState.Clear();

            // once all processing is done, initialise and return the View
            // make sure this is handled in javascript
            return PartialView("_LoginPartial", credentials); // return View("Index", credentials); // do I even need this, or just PartialView??
        }

        [HttpGet]
        [Route("Authenticate")]
        public IActionResult Login()
        {
            // get values from the correct fields
            // string username = Request.Form["txtLoginUsername"].ToString() ?? "";
            // string password = Request.Form["txtLoginPassword"].ToString() ?? "";
            // USE MODEL INSTEAD


            return PartialView();
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }



        // ##################### OTHER ACTIONS ##################### \\
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
