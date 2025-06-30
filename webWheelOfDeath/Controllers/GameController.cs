using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webWheelOfDeath.Models;
using Microsoft.AspNetCore.Http;
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


        #region INDEX

        public IActionResult Index()
        {
            // Communicate to the view whether the user is logged in or not -- so it knows which content to show
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("player-user-id") != null;
            return View("Index");
        }

        #endregion


        #region ACCOUNT/AUTH

        public IActionResult Login()
        {
            // Accessed from other shared partials (i.e. _LoginAndRegister)
            // for the sake of knowing which controller to hand over to.
            ViewData["Controller"] = "Game";

            return PartialView("_LoginAndRegister", new AccountViewModel());
        }

        [HttpPost]
        public IActionResult Authenticate(CredentialsViewModel vm) // take in the shared ViewModel
        {
            try
            {
                // Transfer data from shared ViewModel --> the specific account type Model
                CPlayerCredentials creds = new CPlayerCredentials
                {
                    Username = vm.Username,
                    Password = vm.Password
                };

                // Attempt authentication using the Model.
                creds.Authenticate();

                if (!creds.loginAttemptFailed)
                {

                    // Set the "player-user-id" session variable to the player id (DB field)
                    HttpContext.Session.SetString("player-user-id", creds.Id.ToString());
                    HttpContext.Session.SetString("player-user-name", creds.Username);

                    // CLEAR THE MODELSTATE ARRGGGGGGGGHHH
                    ModelState.Clear();

                    ViewBag.IsLoggedIn = true;


                    // User login success - return the _GameSelection partial!
                    ViewBag.GameSelected = false;
                    try
                    {
                        return PartialView("_GameSelection"); // PartialView("_GameSelection");
                    }
                    catch (Exception ex)
                    {
                        return Content($"Error loading game selection: {ex.Message} - Inner: {ex.InnerException?.Message}");
                    }
                }
                else
                {
                    ModelState.Clear();
                    vm.LastLoginFailed = "Login failed";
                    return PartialView("_LoginAndRegister", vm);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Authentication error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Register(AccountViewModel vm)
        {
            CGameUser player = new CGameUser
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Username = vm.Username,
                Password = vm.Password
            };

            if (player.UsernameExists())
            {
                ModelState.Clear();
                vm.LastRegisterFailed = "Username is already taken";
                return PartialView("_LoginAndRegister", vm);
            }

            // register actions
            player.Register();

            // extract the Credentials base from the player object and return the user to the login page.
            return PartialView("_LoginPartial");
        }

        #endregion

        #region GAME

        [HttpPost]
        public IActionResult Game(long gameId)
        {
            ViewBag.GameSelected = true;
            // Create a gameDifficulty (performs backend DB search)
            var game = new CWebGame(gameId);

            // Return the _Game partial populated with data from a "gameDifficulty" model
            return PartialView("_Game", game);
        }
        public IActionResult GameSelection()
        {
            if (!UserLoggedIn())
                return PartialView("_LoginAndRegister", new AccountViewModel());

            ViewBag.GameSelected = false;
            return PartialView("_GameSelection");
        }

        #endregion



        #region REPORTS

        public IActionResult HallOfFameHigh()
        {
            if (!UserLoggedIn()) return PartialView("_LoginAndRegister", new AccountViewModel());

            CHallOfFame report = new();
            IEnumerable<CHallOfFame> reports = report.TopReport();
            ViewBag.IsHallOfFameAsc = true; // used in the _HallOfFameReport partial to change the title, etc.
            return PartialView("_HallOfFameReport", reports);

        }
        public IActionResult HallOfFameLow()
        {
            if (!UserLoggedIn()) return PartialView("_LoginAndRegister", new AccountViewModel());

            CHallOfFame report = new();
            IEnumerable<CHallOfFame> reports = report.BottomReport();
            ViewBag.IsHallOfFameAsc = false;
            return PartialView("_HallOfFameReport", reports);
        }

        #endregion


        #region HELPERS

        public bool UserLoggedIn()
        {
            ViewBag.IsLoggedIn = (HttpContext.Session.GetString("player-user-id") != null);
            return ViewBag.IsLoggedIn;
        }
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

        #endregion
    }
}
