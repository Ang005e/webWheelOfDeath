using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Controllers
{
    public abstract class BaseController : Controller
    {
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
