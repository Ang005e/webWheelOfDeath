namespace webWheelOfDeath.Exceptions
{
    /// <summary>
    /// Exception thrown when user credentials do not match any account.
    /// </summary>
    public class IncorrectCredentialsException : AuthenticationFailureException
    {
        public IncorrectCredentialsException() : base("The provided details do not match an account.") { }
    }
}