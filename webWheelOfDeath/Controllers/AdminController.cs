using Microsoft.AspNetCore.Mvc;
using webWheelOfDeath.Models;

namespace webWheelOfDeath.Controllers
{

    // ##################### GET ACTIONS ##################### \\
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return PartialView("_LoginAndRegister");
        }




        // ##################### ACCOUNT ACTIONS ##################### \\

        [HttpPost]
        public IActionResult Authenticate(CAdminCredentials adminLogin)
        {
            // Request.Form[""];

            // attempt authentication.
            adminLogin.Authenticate();

            string viewName = "";

            if (!adminLogin.loginAttemptFailed)
            {
                //ToDo: SET USING ID, NOT USERNAME
                HttpContext.Session.SetString("user-id", adminLogin.txtAdminUsername);
                HttpContext.Session.SetString("user-name", adminLogin.txtAdminUsername);

                HttpContext.Session.SetString("previous-login-failed", "false");

                viewName = "_AdminCentre";  // User login success - return the _AdminCentre partial!
            }
            else
            {
                HttpContext.Session.SetString("previous-login-failed", "true");

                viewName = "_LoginAndRegister";
            }

            // CLEAR THE MODELSTATE
            ModelState.Clear();

            return PartialView(viewName);
        }

        [HttpPost]
        public IActionResult Register(CAdminUser admin)
        {
            admin.Register();
            // extract the Credentials base from the player object and return the user to the login page.
            return PartialView("_LoginPartial", (CAdminCredentials)admin);
        }
    }
}
