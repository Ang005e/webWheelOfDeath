namespace webWheelOfDeath.Models.Infrastructure
{
    public interface IWebCredentials
    {
        string Username { get; set; }
        string Password { get; set; }
        // public bool Authenticate();
    }
}
