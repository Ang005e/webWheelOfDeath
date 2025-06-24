using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webWheelOfDeath.Models;
using webWheelOfDeath.Models.ViewModels;

namespace webWheelOfDeath.Controllers
{
    // controllers are containers for actions.
    // every public method in a controller class is an action.

    // controller doesn't HAVE to inherit from Controller.

    // temporary, doesnt survive round trips

    public class GameController : Controller
    {
        #region comments
        // private readonly ILogger<GameController> _logger;

        // ##################### GENERAL ACTIONS ##################### \\
        // every controller action returns a result (ContentResult)
        //public GameController(ILogger<GameController> logger)
        //{
        //    _logger = logger;
        //}

        // how to pass data to controller actions: use parameters

        // routing
        // when the user enters a url named [controller]/post/[value], [value] will be returned as a contentresult
        // so, when navigating to any controller in a url, actions within that controller can
        // be navigated to within that... and they will be run!!
        //public IActionResult Post(string id)
        //{
        //    // contentResult:
        //    // the expected return value from an action
        //    return new ContentResult { Content = id };
        //}

        #endregion

        // ##################### GET ACTIONS ##################### \\

        [HttpGet]
        //[Route("Login")]
        public IActionResult Login()
        {
            return PartialView("_LoginAndRegister");
        }
        [HttpGet]
        // [Route("Index")]
        public IActionResult Index()
        {
            // define all controller logic when it's time to process requests
            return View("Index");
        }

        // ##################### ACCOUNT ACTIONS ##################### \\

        [HttpPost]
        //[Route("Authenticate")]
        public IActionResult Authenticate(CredentialsViewModel vm)
        {
            // Request.Form[""];
             
            CPlayerCredentials creds = new CPlayerCredentials 
            { 
                txtPlayerUsername = vm.Username,
                txtPlayerPassword = vm.Password
            };

            // attempt authentication.
            creds.Authenticate();

            string viewName = "";

            if (!creds.loginAttemptFailed)
            {

                // Set the "user-id" session variable to the player id (DB field)
                //      Meaning, I'll need to get the ID from the CRUDS classes and pass through the model.

                //ToDo: SET USING ID, NOT USERNAME
                HttpContext.Session.SetString("user-id", creds.txtPlayerUsername);
                HttpContext.Session.SetString("user-name", creds.txtPlayerUsername);
                
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
        // [Route("Register")]
        public IActionResult Register(CGameUser player)
        {

            // register actions
            player.Register();

            // extract the Credentials base from the player object and return the user to the login page.
            return PartialView("_LoginPartial", (CPlayerCredentials)player);
        }




        // ##################### GAME ACTIONS ##################### \\
        [HttpPost]
        public IActionResult Game(long gameId)
        {
            // Create a gameDifficulty (performs backend DB search)
            var game = new CWebGame(gameId);

            // Return the _Game partial populated with data from a "gameDifficulty" model
            return PartialView("_Game", game);
        }



        // ##################### MISC ACTIONS ##################### \\
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
