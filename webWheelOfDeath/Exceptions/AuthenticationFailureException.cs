namespace webWheelOfDeath.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication fails.
    /// </summary>
    public class AuthenticationFailureException : ApplicationException
    {
        /// <summary>
        /// New instance of the <see cref="AuthenticationFailureException"/> class with a specified reason for the failure.
        /// For use when an error occurs during the authentication process that is thrown intentionally (i.e. credential mismatch).
        /// </summary>
        /// <param name="reason">The reason for the authentication failure.</param>
        public AuthenticationFailureException(string reason) : base($"An authentication failure occured. Reason: {reason}") { }

        /// <summary>
        /// New instance of the <see cref="AuthenticationFailureException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// For use when an error occurs during the authentication process that is not thrown intentionally (i.e. a null error ).
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public AuthenticationFailureException(string message, Exception innerException) : base($"An error occured while authenticating", innerException) { }
    }
}
