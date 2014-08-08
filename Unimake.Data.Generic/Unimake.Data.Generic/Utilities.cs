using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Unimake.Data.Generic.Model;
using Unimake.Data.Generic.Schema;
using Unimake.Data.Generic.Definitions;

namespace Unimake.Data.Generic
{
    internal static class Utilities
    {
        /// <summary>
        /// Retorna true se o valor for  numérico
        /// </summary>
        /// <param name="value">Valor a ser validado</param>
        /// <returns></returns>
        internal static bool IsNumeric(string value)
        {
            Regex reNum = new Regex(@"^\d+$");
            return reNum.Match(value).Success;
        }

        /// <summary>
        /// Prepara o valor antes de exibir ao usuário
        /// </summary>
        /// <typeparam name="T">Tipo de valor esperado</typeparam>
        /// <param name="value">valor para ser convertido</param>
        /// <returns></returns>
        internal static T PrepareValue<T>(object value)
        {
            T result = default(T);
            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            if(value != null && value != DBNull.Value)
            {
                if(t.IsEnum)
                {
                    int res = Convert.ToInt32(value);
                    result = (T)Enum.Parse(t, res.ToString());
                }
                else if(t.FullName.Equals(typeof(System.String).FullName))
                {
                    string s = value.ToString();

                    result = (T)Convert.ChangeType(s, t);

                }
                else
                {
                    try
                    {
                        result = (T)Convert.ChangeType(value, t);
                    }
                    catch(FormatException)
                    {
                        result = default(T);
                    }
                }
            }

            return result;
        }

        #region DbUtils
        internal static class DbUtils
        {
            internal static IEnumerable<MyHierarchicalType> GetModels(IBaseModel model)
            {
                Type type = model.GetType();
                int i = 0;
                for(var current = type; current != null &&
                                        !current.FullName.Equals(typeof(System.Object).FullName);
                    current = current.BaseType)
                    yield return new MyHierarchicalType()
                    {
                        Order = i++,
                        Type = current
                    };
            }

            internal static IID GetPrimaryKeyValue(IBaseModel model)
            {
                throw new NotImplementedException();
            }

            static Tables _tables;
            internal static Tables Tables
            {
                get
                {
                    if(_tables == null)
                        _tables = Tables.GetTables(DbContext.CreateConnection());

                    return _tables;
                }
            }
        }
        #endregion
    }
}
