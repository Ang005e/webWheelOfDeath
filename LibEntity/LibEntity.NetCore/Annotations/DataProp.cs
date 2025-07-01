using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibEntity.NetCore.Annotations
{
    /// <summary>
    /// Marker attribute, for application to properties that interact with database fields. 
    /// Decorated properties will be assigned a default value based on type, via a call (for 
    /// all decorated DataProp members) to <see cref="GetDefaultPropVal{T}"/> upon 
    /// initialisation of <seealso cref="CEnity"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataProp : Attribute { }
}
