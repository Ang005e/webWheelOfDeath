using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace webWheelOfDeath.Exceptions
{
    /// <summary>
    /// Exception thrown when an attempt is made to access a property that has not been set.
    /// </summary>
    public class UnsetPropertyException<TUnsetProperty> : InvalidOperationException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsetValueException"/> class with a specified property or value name.
        /// </summary>
        /// <param name="property">The property that was not set.</param>
        public UnsetPropertyException(TUnsetProperty? property, [CallerArgumentExpression("property")] string? propertyName = null)
            : base($"The value for '{propertyName??"(property name not provided)"}' has not been set.")
        {}
    }
}
