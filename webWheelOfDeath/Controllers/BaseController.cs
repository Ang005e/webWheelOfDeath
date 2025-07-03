using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Controllers
{
    /// <summary>
    /// Base controller class, with shared functionality for both pages. 
    /// All controllers must inherit.
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Adds a feedback message to be displayed to the user. 
        /// Adds the feedback to the response headers for AJAX requests.
        /// For compatablility with my old system, it also stores in TempData and 
        /// ViewBag--though these features are no longer strictly needed.
        /// Intended for user feedback for actions such as login, registration, 
        /// game management, and error handling.
        /// </summary>
        /// <param name="message">The feedback message to display to the user.</param>
        /// <param name="type">The type of feedback (Info, Success, Warning, Error). Defaults to Info.</param>
        protected void AddFeedback(string message, EnumFeedbackType type = EnumFeedbackType.Info)
        {
            var feedback = new FeedbackMessage(type, message);

            // Store in TempData for redirects
            TempData["_Feedback"] = JsonSerializer.Serialize(feedback);

            // Store in ViewBag for current request
            ViewBag._Feedback = feedback;

            // For AJAX requests, add to response headers
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Response.Headers.Append("X-Feedback", JsonSerializer.Serialize(feedback));
            }
        }
    }
}
