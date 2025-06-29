using LibWheelOfDeath.Exceptions;
using Microsoft.AspNetCore.Mvc;
using webWheelOfDeath.Models;
using webWheelOfDeath.Models.ViewModels;

namespace webWheelOfDeath.Controllers
{

    public class AdminController : Controller
    {

        #region GET ACTIONS
        public IActionResult Index()
        {
            // Communicate to the view whether the user is logged in or not -- so it knows which content to show.
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("admin-user-id") != null;

            return View("Index");
        }

        public IActionResult AdminCentre()
        {
            return PartialView("_AdminCentre");
        }

        public IActionResult Login()
        {
            // Accessed from other shared partials (i.e. _LoginAndRegister)
            // for the sake of knowing which controller to hand over to.
            ViewData["Controller"] = "Admin";
            
            return PartialView("_LoginAndRegister", new CredentialsViewModel());
        }
        #endregion



        #region ACCOUNT ACTIONS

        [HttpPost]
        public IActionResult Authenticate(CredentialsViewModel vm)
        {

            CAdminCredentials creds = new CAdminCredentials
            {
                Username = vm.Username,
                Password = vm.Password
            };

            // Attempt authentication.
            creds.Authenticate();

            if (!creds.loginAttemptFailed)
            {

                // Set the "player-user-id" session variable to the player id (DB field)
                HttpContext.Session.SetString("admin-user-id", creds.Id.ToString());
                HttpContext.Session.SetString("admin-user-name", creds.Username);

                // CLEAR THE MODELSTATE ARRGGGGGGGGHHH
                ModelState.Clear();
                
                // Admin login success.
                return PartialView("_AdminCentre");
            }
            else
            {
                ModelState.Clear();
                vm.LastLoginFailed = "Login failed"; // ToDo: make sure error messages are less helpful--users
                                                     // MUST NOT know why they cannot log in.
                                                     // (this is called sarcasm)
                return PartialView("_LoginAndRegister", vm);
            }

        }


        [HttpPost]
        public IActionResult Register(AccountViewModel vm)
        {
            CAdminUser admin = new CAdminUser
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Username = vm.Username,
                Password = vm.Password
            };

            if (admin.UsernameExists())
            {
                ModelState.Clear();
                vm.LastRegisterFailed = "Username is already taken";
                return PartialView("_LoginAndRegister", vm);
            }

            // register actions
            admin.Register();

            // extract the Credentials base from the player object and return the user to the login page.
            return PartialView("_LoginPartial");
        }

        #endregion



        #region MANAGEMENT ACTIONS

        [HttpGet]
        public IActionResult CreateGame()
        {
            var diffs = CWebGameDifficulty.GetDifficulties();
            ViewBag.Difficulties = diffs.Count() > 0 ? diffs : new List<CWebGameDifficulty>();
            return PartialView("_CreateGame", new CWebGame());
        }

        [HttpPost]
        public IActionResult CreateGame(CWebGame gm)
        {
            try
            {
                gm.Create();
                TempData["LastUserActionSuccess"] = true;
                TempData["LastUserAction"] = "New gamemode created!";
            }
            catch (CWheelOfDeathException E)
            {
                TempData["LastUserActionSuccess"] = false;
                TempData["LastUserAction"] = "Error creating gamemode: " + E.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateAdminAccount()
        {
            return PartialView("_CreateAdminAccount", new CAdminUser());
        }

        [HttpPost]
        public IActionResult CreateAdminAccount(CAdminUser admin)
        {
            try
            {
                admin.Register();
                TempData["LastUserActionSuccess"] = true;
                TempData["LastUserAction"] = "New admin registered!";
            }
            catch (CWheelOfDeathException E)
            {
                TempData["LastUserActionSuccess"] = false;
                TempData["LastUserAction"] = "Error registering admin: " + E.Message;
            }
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public void EditAdminAccount(CAdminUser admin)
        //{
        //    try
        //    {
        //        admin.Edit();
        //    }
        //    catch (CWheelOfDeathException E)
        //    {
        //    }
        //}

        #endregion

    }
}
