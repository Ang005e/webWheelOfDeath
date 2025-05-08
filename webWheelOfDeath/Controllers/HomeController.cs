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
        public IActionResult Login()
        {
            return PartialView("_LoginAndRegister");
        }

        [HttpPost]
        [Route("Authenticate")]
        public IActionResult Authenticate(CCredentials playerLogin)
        {
            // Request.Form[""];

            // attempt authentication.
            playerLogin.Authenticate();

            string viewName = "";

            if (!playerLogin.loginAttemptFailed)
            {

                // Set the "user-id" session variable to the player id (DB field)
                //      Meaning, I'll need to get the ID from the CRUDS classes and pass through the model.

                //ToDo: SET USING ID, NOT USERNAME
                HttpContext.Session.SetString("user-id", playerLogin.txtPlayerUsername);
                HttpContext.Session.SetString("user-name", playerLogin.txtPlayerUsername);

                HttpContext.Session.SetString("previous-login-failed", "false");

                viewName = "_GameSelection";  // User login success - return the _GameSelection partial!
            }
            else
            {
                HttpContext.Session.SetString("previous-login-failed", "true");

                viewName = "_LoginAndRegister";
            }

            // CLEAR THE MODELSTATE ARRGGGGGGGGHHH
            ModelState.Clear();

            return PartialView(viewName);
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(CGameUser player)
        {

            // register actions
            player.Register();

            // extract the Credentials base from the player object and return the user to the login page.
            return PartialView("_LoginPartial", (CCredentials)player);
        }

        [HttpPost]
        [Route("UserEntry")]
        public IActionResult UserEntry()
        {
            return PartialView("_LoginAndRegister");
        }

        [HttpPost]
        [Route("Game")]
        public IActionResult Game(long gameId)
        {
            // Create a gameDifficulty (performs backend DB search)
            var game = new CWebGame(gameId);

            // Return the _Game partial populated with data from a "gameDifficulty" model
            return PartialView("_Game", game);
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
