
namespace webWheelOfDeath.Models.Infrastructure
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public required T Data { get; set; }
        public required FeedbackMessage Feedback { get; set; }
    }
}
