using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LibEntity.NetCore.Annotations;

namespace LibEntity.NetCore.Infrastructure
{
    public static class CDataDefaults
    {

        /// <summary>
        /// A Dictionary that contains the default initialisation values for common property types.
        /// For the sake of clean handling of default values, and prevention of defaults being written to the DB.
        /// </summary>
        public static Dictionary<Type, object?> PropertyTypeDefaults { get; } = new Dictionary<Type, object?>
        {
            {typeof(int), 0},
            { typeof(short), 0},
            { typeof(long), 0L },
            { typeof(float), 0F },
            { typeof(double), 0D },
            { typeof(string), "" },
            { typeof(bool), false },
        };

        /// <summary>
        /// Return the "framework default" for the requested type.
        /// </summary>
        /// <typeparam name="T">The property type for which to get the default value.</typeparam>
        /// <returns>The default value for the specified type T.</returns>
        /// <remarks>Please note that this artifact was generated with assistance from AI tools</remarks>
        public static T FetchDefaultTypeValue<T>()
        {
            Type propType = typeof(T);

            // if the property itself is Nullable<TBase>, default(T) is null
            Type? nullableUnderlying = Nullable.GetUnderlyingType(propType);
            if (nullableUnderlying != null)
                return default!;                        // null for int?, bool?, etc

            // default(T) is the 0-value of enum
            if (propType.IsEnum)
                return default!;   

            // look for an override in the dictionary
            if (PropertyTypeDefaults.TryGetValue(propType, out var boxed)
                && boxed is T typed)
                return typed;

            // default(T)
            return default!;
        }


        /// <summary>  
        /// Extension method to get the default value of a property by its name.  
        /// </summary>  
        /// <typeparam name="T">The type of the property.</typeparam>  
        /// <param name="target">The object containing the property.</param>  
        /// <param name="propName">The name of the property.</param>  
        /// <returns>The default value of the property.</returns>  
        public static T GetPropertyDefault<T>(object target, string propName)
        {
            // Get the property info using reflection  
            PropertyInfo? propInfo = target.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // propInfo must not be null
            if (propInfo == null)
            {
                throw new ArgumentException($"Property '{propName}' not found on type '{target.GetType().Name}'.");
            }

            // Fetch the default value for the property type  
            return FetchDefaultTypeValue<T>();
        }


        // other stuff 

        private static readonly MethodInfo _open = typeof(CDataDefaults).GetMethod(nameof(CDataDefaults.GetPropertyDefault))!;

        public static bool IsDefaultValue(this object target, string propertyName)
        {
            var p = target.GetType().GetProperty(propertyName)
                  ?? throw new ArgumentException($"Could not find property '{propertyName}'");

            if (!Attribute.IsDefined(p, typeof(DataProp)))
                return false; // not of DataProp, no default to compare

            var closed = _open.MakeGenericMethod(p.PropertyType);

            var defValue = closed.Invoke(null, new object[] { target, propertyName });

            var current = p.GetValue(target);
            return Equals(current, defValue);
        }
        // overload where I can just pass the param staight in using CallerArgumentExpression
        public static bool IsDefaultValue<TProp>(this object target, TProp property, [CallerArgumentExpression("property")] string propName = "")
        {
            return IsDefaultValue(target, propName);
        }
        public static bool NotDefaultValue<TProp>(this object target, TProp property, [CallerArgumentExpression("property")] string propName = "")
        {
            return ! IsDefaultValue(target, propName);
        }
    }
}
