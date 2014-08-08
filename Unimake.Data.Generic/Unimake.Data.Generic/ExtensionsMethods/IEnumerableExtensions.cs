using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Unimake
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Concatena strings utilizando separador.
        /// </summary>
        /// <param name="values">Valores a serem concatenados.</param>
        /// <param name="separator">Separador.</param>
        /// <returns></returns>
        public static string Join(this IEnumerable values, char separator = ',')
        {
            if(values == null) return "";

            string result = "";

            foreach(var item in values)
                result += string.Format("{0}{1}", item, separator);

            if(result.Length > 0)
                result = result.Substring(0, result.Length - 1);

            return result;
        }

        /// <summary>
        /// Verifica se a lista está vazia.
        /// </summary>
        /// <param name="value">Lista informada;</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IList<T> value)
        {
            return value == null || value.Count == 0 ? true : false;
        }
    }
}
