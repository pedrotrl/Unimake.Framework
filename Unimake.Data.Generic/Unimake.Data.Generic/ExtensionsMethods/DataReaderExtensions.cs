using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unimake.Data.Generic.Exceptions;
using System.Data;

namespace Unimake.Data.Generic
{
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Recupera o valor do registro e retorna
        /// </summary>
        /// <typeparam name="T">Tipo de retorno</typeparam>
        /// <param name="dataReader">dataReader instanciado</param>
        /// <param name="name">nome do campo do no registro</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dataReader, string name)
        {
            int index = dataReader.GetOrdinal(name);

            if(index == -1) throw new FieldDoesntExist(name);

            return dataReader.GetValue<T>(index);
        }

        /// <summary>
        /// Recupera o valor do registro e retorna
        /// </summary>
        /// <typeparam name="T">Tipo de retorno</typeparam>
        /// <param name="dataReader">dataReader instanciado</param>
        /// <param name="index">índice do valor no registro</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dataReader, int index)
        {
            return Utilities.PrepareValue<T>(dataReader.GetValue(index));
        }
    }
}
