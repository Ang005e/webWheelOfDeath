using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Login(CCredentials playerLogin)
        {

            // attempt authentication.
            playerLogin.Authenticate();

            HttpContext.Session.SetString("previous-login-attempted", "true");

            if ( ! playerLogin.loginAttemptFailed)
            {

                // Set the "user-id" session variable to the player id (DB field)
                //      Meaning, I'll need to get the ID from the CRUDS classes and pass through the model.

                //ToDo: SET USING ID, NOT USERNAME
                HttpContext.Session.SetString("user-id", playerLogin.txtPlayerUsername);
                HttpContext.Session.SetString("user-name", playerLogin.txtPlayerUsername);

                return PartialView("_Game");  // User login success - return the Game!
            }

            // CLEAR THE MODELSTATE ARRGGGGGGGGHHH
            ModelState.Clear();

            return PartialView("_LoginPartial");
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(CGameUser player)
        {
            throw new NotImplementedException("Register action has not been implemented");
            // return PartialView("_LoginPartial"); // return user to the login page (if regitration succeeds)
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
