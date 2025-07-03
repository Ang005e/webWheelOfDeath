using System.Text.Json;
using LibEntity;
using LibWheelOfDeath;
using LibWheelOfDeath.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using webWheelOfDeath.Exceptions;
using webWheelOfDeath.Models;
using webWheelOfDeath.Models.Infrastructure;
using webWheelOfDeath.Models.ViewModels;

namespace webWheelOfDeath.Controllers
{

    public class AdminController : BaseController
    {

        #region GET ACTIONS
        public IActionResult Index()
        {
            // Accessed from other shared partials (i.e. _LoginAndRegister)
            // for the sake of knowing which controller to hand over to.
            // HttpContext.Session.SetString("Controller", "Admin"); setting in ViewStart now

            // Communicate to the view whether the user is logged in or not -- so it knows which content to show.
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("admin-id") != null;


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

            bool? loginSuccess = null;

            try
            {
                // Attempt authentication.
                loginSuccess = admin.Authenticate();
            }
            catch (AuthenticationFailureException ex) 
            {
                AddFeedback($"Login failed: {ex.Reason}", EnumFeedbackType.Error);
                loginSuccess = false;
            }

            if (loginSuccess??false)
            {
                // get Id
                admin.BuildEntity();

                // Set the "player-id" session variable to the player id (DB field)
                HttpContext.Session.SetString("admin-id", admin.Id.ToString());
                HttpContext.Session.SetString("admin-user-name", admin.Username);

                // CLEAR THE MODELSTATE ARRGGGGGGGGHHH
                ModelState.Clear();

                AddFeedback($"Welcome back, {admin.Username}!");
                
                // Admin login success.
                return PartialView("_AdminCentre");
            }

            ModelState.Clear();
            // never happens:
            if (loginSuccess == null) AddFeedback("Unknown failure during authentication; please contact an administrator.", EnumFeedbackType.Error);
            return PartialView("_LoginAndRegister", vm);

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
            return long.Parse(HttpContext.Session.GetString("admin-id")??"0");
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
                AddFeedback("New gamemode created!", EnumFeedbackType.Success);
            }
            catch (CWheelOfDeathException E)
            {
                AddFeedback("Error creating gamemode: " + E.Message, EnumFeedbackType.Error);
            }

            return PartialView("_AdminCentre");
        }

        [HttpGet]
        public IActionResult ManageGameState()
        {
            if (!IsSuperAdmin()) return DenyAccess(EnumAdminType.Admin, "manage games");

            IEnumerable<CWebGame> games = new CWebGame().GetGames();

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
            IEnumerable<CWebGame> games = new CWebGame().GetGames();
            try
            {
                CGame game = new CGame(id);
                game.Read();
                game.IsActiveFlag = !game.IsActiveFlag;
                game.Update();

                AddFeedback($"Game is now {(game.IsActiveFlag ? "active" : "inactive")}", EnumFeedbackType.Success);
                return PartialView("_ListGames", games);

            }
            catch (Exception ex)
            {
                AddFeedback("Error updating game: " + ex.Message, EnumFeedbackType.Error);
                return PartialView("_ListGames", games);
            }
        }

        // ToDo: remove, unused
        [HttpPost]
        public IActionResult ManageGame(CWebGame g)
        {
            g.SetActive(g.IsActiveFlag);

            AddFeedback($"Game is now {(g.IsActiveFlag ? "active" : "inactive")}", EnumFeedbackType.Success);
            return PartialView("_AdminCentre");
        }

        #endregion


        #region ADMIN MANAGEMENT ACTIONS

        [HttpGet]
        public IActionResult ListAdminAccounts()
        {
            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();
            return PartialView("_ListAdminAccounts", admins);
        }

        //
        // Create an admin
        //
        // Get the correct partial page
        [HttpGet]
        public IActionResult CreateAdminAccount() => PartialView("_CreateAdminAccount", new CAdminUser());


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
                AddFeedback("New admin registered!", EnumFeedbackType.Success);
            }
            catch (CWheelOfDeathException E)
            {
                AddFeedback("Error registering admin: " + E.Message, EnumFeedbackType.Error);
            }
            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();
            return PartialView("_ListAdminAccounts", admins);
        }

        // On post, create the admin account.
        [HttpPost]
        public IActionResult ToggleAdminActive(long id)
        {
            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();

            if (MatchesLoggedAdmin(id))
            {
                AddFeedback("You cannot deactivate your own account", EnumFeedbackType.Warning);
                return PartialView("_ListAdminAccounts", admins);
            }

            CAdminUser user = new CAdminUser(id);

            user.IsActive = !user.IsActive;
            user.Update();

            AddFeedback($"Admin account {(user.IsActive ? "activated" : "deactivated")}", EnumFeedbackType.Success);

            return PartialView("_ListAdminAccounts", admins);
        }

        // There's no [HttpGet] method overload for DeleteAdminAccount, since the delete post
        // is sent directly from the _ListAdminAccounts partial (via the delete button).
        [HttpPost]
        public IActionResult DeleteAdminAccount(long id)
        {

            List<CAdminUser> admins = new CAdminUser().GetAllAdmins();

            if (id == long.Parse(HttpContext.Session.GetString("admin-id") ?? "0"))
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


        #region PLAYER MANAGEMENT ACTIONS


        [HttpGet]
        public IActionResult CreatePlayerAccount() => PartialView("_CreatePlayerAccount", new CGameUser());


        [HttpPost]
        public IActionResult CreatePlayerAccount(CGameUser player)
        {
            try
            {
                player.Register();
                AddFeedback("New player registered!", EnumFeedbackType.Success);
            }
            catch (CWheelOfDeathException E)
            {
                AddFeedback("Error registering player: " + E.Message, EnumFeedbackType.Error);
            }
            List<CGameUser> players = new CGameUser().GetAllPlayers();
            return PartialView("_ListPlayerAccount", players);
        }

        [HttpGet]
        public IActionResult ListPlayerAccount()
        {
            var players = new CGameUser().GetAllPlayers();
            return PartialView("_ListPlayerAccount", players);
        }


        [HttpPost]
        public IActionResult TogglePlayerActive(long id)
        {
            try
            {
                CGameUser player = new CGameUser(id);
                player.IsActive = !player.IsActive;
                player.Update();

                AddFeedback($"Player account {(player.IsActive ? "activated" : "deactivated")}", EnumFeedbackType.Success);
            }
            catch (Exception ex)
            {
                AddFeedback("Error updating player: " + ex.Message, EnumFeedbackType.Error);
            }

            var players = new CGameUser().GetAllPlayers();
            return PartialView("_ListPlayerAccount", players);
        }

        [HttpPost]
        public IActionResult DeletePlayerAccount(long id)
        {
            var players = new CGameUser().GetAllPlayers();
            try
            {
                // Check if player has game records first
                CGameRecord searchRecord = new() { FkPlayerId = id };
                if (searchRecord.Search().Any())
                {
                    AddFeedback("Cannot delete player with existing game records. Deactivate their account instead.", EnumFeedbackType.Warning);
                    return PartialView("_ListPlayerAccount", players);
                }

                CGameUser player = new CGameUser(id);
                player.Delete();
                AddFeedback("Player account deleted", EnumFeedbackType.Success);
            }
            catch (Exception ex)
            {
                AddFeedback("Error deleting player: " + ex.Message, EnumFeedbackType.Error);
            }

            return PartialView("_ListPlayerAccount", players);
        }

        [HttpGet]
        public IActionResult EditPlayerAccount(long id)
        {
            CGameUser player = new CGameUser(id);
            return PartialView("_EditPlayerAccount", player);
        }

        [HttpPost]
        public IActionResult EditPlayerAccount(CGameUser player)
        {
            try
            {
                player.Update();
                AddFeedback("Player account updated!", EnumFeedbackType.Success);
            }
            catch (Exception ex)
            {
                AddFeedback("Error updating player: " + ex.Message, EnumFeedbackType.Error);
            }

            var players = new CGameUser().GetAllPlayers();
            return PartialView("_ListPlayerAccount", players);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Checks if the provided ID matches the ID of the currently logged account.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool MatchesLoggedAdmin(long id) {
            return (id == long.Parse(HttpContext.Session.GetString("admin-id") ?? "0"));
        }

        #endregion
    }
}
