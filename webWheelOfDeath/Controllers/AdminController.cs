using System.Text.Json;
using LibEntity;
using LibWheelOfDeath;
using LibWheelOfDeath.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using webWheelOfDeath.Models;
using webWheelOfDeath.Models.Infrastructure;
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
            // removed because it's the job of a ViewModel, not an interface
            //IWebCredentials creds = new CAdminUser() // we dont need or want to know about the rest, only IWebCredentials
            //{
            //    Username = vm.Username,
            //    Password = vm.Password
            //};

            //// Attempt authentication.
            //bool loginSuccess = creds.Authenticate();

            CAdminUser admin = new CAdminUser() // we dont need or want to know about the rest, only IWebCredentials
            {
                Username = vm.Username,
                Password = vm.Password
            };

            // Attempt authentication.
            bool loginSuccess = admin.Authenticate();

            if (loginSuccess)
            {
                // Set the "player-user-id" session variable to the player id (DB field)
                HttpContext.Session.SetString("admin-user-id", admin.Id.ToString());
                HttpContext.Session.SetString("admin-user-name", admin.Username);

                // CLEAR THE MODELSTATE ARRGGGGGGGGHHH
                ModelState.Clear();
                
                // Admin login success.
                return PartialView("_AdminCentre");
            }
            else
            {
                ModelState.Clear();
                vm.Password = ""; // clear password
                AddFeedback("Username or password were incorrect", EnumFeedbackType.Warning);
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
                vm.Password = ""; // clear password
                AddFeedback("Username is already taken", EnumFeedbackType.Warning);
                return PartialView("_LoginAndRegister", vm);
            }

            // register actions
            admin.Register();

            return PartialView("_LoginPartial", vm);
        }

        public long GetLoggedId()
        {
            return long.Parse(HttpContext.Session.GetString("admin-user-id")??"0");
        }

        public bool IsSuperAdmin()
        {
            long adminId = GetLoggedId();
            if (adminId <= 0) return false; // not logged in...?

            CAdminUser admin = new(adminId);

            return admin.AdminTypeId == 2; // EnumAdminType.SuperAdmin; 
        }
        public IActionResult DenyAccess(EnumAdminType denyAtLevel, string attemptedAction)
        {
            AddFeedback($"{denyAtLevel.ToString()}s and below do not have permission to {attemptedAction}");
            return PartialView("_AdminCentre");
        }

        #endregion


        #region GAME MANAGEMENT ACTIONS

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
        public IActionResult ManageGameState()
        {
            if (!IsSuperAdmin()) return DenyAccess(EnumAdminType.Admin, "manage games.");

            List<CWebGame> games = new List<CWebGame>();

            // this is not CGame, this is CWebGame. You have been fooled, you are delusional. 
            // I have perfect seperation of concerns and am not lazy in the slightest.
            CGame gameEntity = new CGame();

            List<IEntity> searchResults = gameEntity.Search();

            foreach (IEntity entity in searchResults)
            {
                CGame g = (CGame)entity;
                games.Add(new CWebGame(g.Id));
            }

            return PartialView("_ListGames", games);
        }

        [HttpGet]
        public IActionResult GameDetail(long id)
        {
            return PartialView("_ManageGameState", new CWebGame(id));
        }

        [HttpPost]
        public IActionResult ToggleGameActive(long id)
        {
            try
            {
                CGame game = new CGame(id);
                game.Read();
                game.IsActiveFlag = !game.IsActiveFlag;
                game.Update();

                TempData["LastUserActionSuccess"] = true;
                TempData["LastUserAction"] = $"Game is now {(game.IsActiveFlag ? "active" : "inactive")}";

                return PartialView("_AdminCentre");
                //return Json(new
                //{
                //    success = true,
                //    isActive = game.IsActiveFlag,
                //    message = $"Game is now {(game.IsActiveFlag ? "active" : "inactive")}."
                //});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ManageGame(CWebGame g)
        {
            g.SetActive(g.IsActiveFlag);

            TempData["LastUserActionSuccess"] = true;
            TempData["LastUserAction"] = $"Game is now {(g.IsActiveFlag ? "active" : "inactive")}";

            return PartialView("_AdminCentre");
            //return Json(new { success = true, message = $"Game is now {(g.IsActiveFlag ? "active" : "invisible")}." });
        }

        #endregion


        #region ADMIN MANAGEMENT ACTIONS

        [HttpGet]
        public IActionResult ListAdminAccounts()
        {
            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();
            return PartialView("_ListAdminAccounts", admins);
        }

        [HttpGet]
        public IActionResult CreateAdminAccount()
        {
            return PartialView("_CreateAdminAccount", new CAdminUser());
        }

        [HttpPost]
        public IActionResult CreateAdminAccount(CAdminUser admin)
        {
            if (!IsSuperAdmin() && (admin.AdminTypeId > 1))
            {
                return DenyAccess(EnumAdminType.Admin, "create super admin accounts");
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
            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();
            return PartialView("_ListAdminAccounts", admins);
        }

        [HttpPost]
        public IActionResult ToggleAdminActive(long id)
        {
            if (MatchesLoggedUser(id))
                return DenyAccess(EnumAdminType.Admin, "manage accounts");

            CAdminUser user = new CAdminUser(id);

            user.IsActive = !user.IsActive;
            user.Update();

            AddFeedback($"Admin account {(user.IsActive ? "activated" : "deactivated")}", EnumFeedbackType.Success);

            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();
            return PartialView("_ListAdminAccounts", admins);
        }

        [HttpPost]
        public IActionResult DeleteAdminAccount(long id)
        {

            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();

            if (id == long.Parse(HttpContext.Session.GetString("admin-user-id") ?? "0"))
            {
                AddFeedback("You cannot delete your own account!", EnumFeedbackType.Warning);
                return PartialView("_ListAdminAccounts", admins);
            }

            CAdminUser user = new CAdminUser(id);
            user.Delete();

            AddFeedback($"Admin account deleted", EnumFeedbackType.Success);
            return PartialView("_ListAdminAccounts", admins);
        }


        #endregion


        #region Helpers

        /// <summary>
        /// Checks if the provided ID matches the ID of the currently logged account.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool MatchesLoggedUser(long id) {
            return (id == long.Parse(HttpContext.Session.GetString("admin-user-id") ?? "0"));
        }

        protected void AddFeedback(string message, EnumFeedbackType type = EnumFeedbackType.Info)
        {
            var feedback = new FeedbackMessage(type, message);

            // Store in multiple places, for compatibility with my old system
            TempData["_Feedback"] = JsonSerializer.Serialize(feedback);
            ViewBag._Feedback = feedback;
        }

        #endregion
    }
}
