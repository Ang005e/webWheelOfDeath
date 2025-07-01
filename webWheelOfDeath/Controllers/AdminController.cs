using LibWheelOfDeath;
using LibWheelOfDeath.Exceptions;
using Microsoft.AspNetCore.Authorization;
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
            // Accessed from other shared partials (i.e. _LoginAndRegister)
            // for the sake of knowing which controller to hand over to.
            // HttpContext.Session.SetString("Controller", "Admin"); setting in ViewStart now

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
            return PartialView("_LoginAndRegister", new CredentialsViewModel());
        }
        #endregion



        #region ACCOUNT/AUTH ACTIONS

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
                vm.LastLoginFailed = "Username or passwoed were incorrect"; 
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

        public bool IsAnyAdmin()
        {
            return HttpContext.Session.GetString("admin-user-id") != null;
        }

        public bool IsSuperAdmin()
        {
            long adminId = long.Parse(HttpContext.Session.GetString("admin-user-id")??"0");

            if (adminId == 0) return false; // Not logged in, so (probably) not a super admin...

            CAdminUser admin = new(adminId);
            
            return admin.AdminType == EnumAdminType.SuperAdmin; 
        }
        public IActionResult DenyAccess(Models.EnumAdminType denyLevel, string attemptedAction)
        {
            TempData["LastUserActionSuccess"] = false;
            TempData["LastUserAction"] = $"{denyLevel.ToString()}s do not have permission to {attemptedAction}.";
            return PartialView("_AdminCentre");
        }

        #endregion


        #region MANAGEMENT ACTIONS

        [HttpGet]
        public IActionResult CreateGame()
        {
            if (!IsSuperAdmin()) return DenyAccess(EnumAdminType.Admin, "create new game modes");

            var diffs = CWebGameDifficulty.GetDifficulties();
            ViewBag.Difficulties = diffs.Count() > 0 ? diffs : new List<CWebGameDifficulty>();
            return PartialView("_CreateGame", new CWebGame());
        }

        [HttpPost]
        public IActionResult CreateGame(CWebGame gm)
        {
            // removed--they'll never get here anyway
            // if (!IsSuperAdmin()) return DenyAccess(EnumAdminType.Admin, "create new game modes");
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

            return PartialView("_AdminCentre");
        }

        [HttpGet]
        public IActionResult CreateAdminAccount()
        {
            return PartialView("_CreateAdminAccount", new CAdminUser());
        }

        [HttpPost]
        public IActionResult CreateAdminAccount(CAdminUser admin)
        {
            if (!IsSuperAdmin() && (admin.AdminType == EnumAdminType.SuperAdmin))
            {
                return DenyAccess(EnumAdminType.Admin, "create Super Admin accounts");
            }
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
            return PartialView("_AdminCentre");
        }

        [HttpGet]
        public IActionResult ManageGames()
        {
            if (!IsSuperAdmin()) return DenyAccess(EnumAdminType.Admin, "manage games.");

            List<CWebGame> games = new List<CWebGame>();

            CGame gameEntity = new CGame();
            var gameRecords = gameEntity.GetAllGames(); 

            foreach (CGame g in gameRecords)
            {
                games.Add(new CWebGame(g.Id));
            }

            return PartialView("_ListGames", games);
        }

        [HttpGet]
        public IActionResult GameDetail(long id)
        {
            return PartialView("_ManageGame", new CWebGame(id));
        }

        [HttpPost] 
        public IActionResult ToggleGameState(long id)
        {
            var g = new CWebGame() { Id = id };
            g.Create();
            return PartialView("_ManageGame", g);
        }
        #endregion

    }
}
