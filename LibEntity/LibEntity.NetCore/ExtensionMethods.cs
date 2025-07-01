using LibEntity.NetCore.Exceptions;
using LibEntity.NetCore.Infrastructure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibEntity
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Extension method to mimic the built-in <c>AddWithValue</c>
        /// method of the <c>SqlCommand</c> object for List&lt;SqlParameter&gt;
        /// objects.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="substitutionSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter AddWithValue(this List<SqlParameter> list, string substitutionSymbol, object? value)
        {
            SqlParameter parameter = new SqlParameter(substitutionSymbol, value);

            list.Add(parameter);

            return parameter;
        }


        /// <summary>
        /// If <paramref name="value"/> contains a database NULL value,
        /// this method returns a C# null.
        /// 
        /// Otherwise, it returns the incoming <paramref name="value"/> unchanged.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object? DeNull(this object? value)
        {
            //if (value == DBNull.Value)
            //{
            //    return null;
            //}
            //else
            //{
            //    return value;
            //}

            return value == DBNull.Value ? null : value;
        }

    }
}
