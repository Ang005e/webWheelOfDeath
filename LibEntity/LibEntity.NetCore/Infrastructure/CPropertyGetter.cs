using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibEntity.NetCore.Annotations;

namespace LibEntity.NetCore.Infrastructure
{
    public static class CPropertyGetter
    {
        /// <summary>
        /// Gets all properties of the specified object that are marked with the <see cref="DataProp"/> attribute.
        /// Only retrieves writable properties (props with a setter).
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(object target)
        {
            // get all properties that are public, non-public, or private.
            IEnumerable<PropertyInfo> props = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                // make sure they're settable
                .Where(prop => prop.CanWrite)
                // only props marked with DataProp
                .Where(prop => prop.GetCustomAttribute<DataProp>() != null);

            return props;

        }
    }
}
