using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webWheelOfDeath.Models;
using Microsoft.AspNetCore.Http;

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
