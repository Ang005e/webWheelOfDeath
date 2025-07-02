namespace webWheelOfDeath.Models.Infrastructure
{
    public enum EnumFeedbackType
    {
        None = 0,
        Success = 1,
        Warning = 2,
        Error = 3,
        Info = 4,
    }
    public class FeedbackMessage
    {
        public EnumFeedbackType Type { get; set; }
        public string Message { get; set; }
        public int? Duration { get; set; }
        public FeedbackMessage(EnumFeedbackType type, string message = "")
        {
            Type = type;
            if (string.IsNullOrWhiteSpace(message) && type != EnumFeedbackType.None)
            {
                throw new ArgumentException($"{nameof(Message)} must be set when ${Type} is not {EnumFeedbackType.None}");
            }
            Message = message;
        }
        public FeedbackMessage(EnumFeedbackType type, string message, int? duration)
        {
            Type = type;
            Message = message;
            Duration = duration;
        }
    }
}
