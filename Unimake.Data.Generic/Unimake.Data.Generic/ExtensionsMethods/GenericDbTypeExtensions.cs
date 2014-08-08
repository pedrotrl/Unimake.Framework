using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic
{
    /// <summary>
    /// Extensões do enum GenericDbType
    /// </summary>
    public static class GenericDbTypeExtensions
    {
        /// <summary>
        /// Detecta o tipo genérico pelo valor informado
        /// </summary>
        /// <param name="type">valor do tipo GenericDbType</param>
        /// <param name="value">valor que será passado ao SGBD</param>
        /// <returns></returns>
        public static GenericDbType DetectTypeFromValue(this GenericDbType type, object value)
        {
            if(value is Enum)
                return GenericDbType.Integer;

            if(value is System.SByte ||
               value is System.Byte)
                return GenericDbType.Byte;

            if(value is System.Int16 ||
               value is System.UInt16 ||
               value is System.Int32 ||
               value is System.UInt32 ||
               value is System.Int64 ||
               value is System.UInt64)
                return GenericDbType.Integer;

            if(value is System.Double ||
               value is System.Decimal ||
               value is System.Single)
                return GenericDbType.Float;

            if(value is System.String ||
               value is System.Char)
                return GenericDbType.String;

            if(value is System.Boolean)
                return GenericDbType.Boolean;

            if(value is System.DateTime)
                return GenericDbType.DateTime;

            return GenericDbType.Unknown;
        }


        /// <summary>
        /// Retorna o Type por este valor
        /// </summary>
        /// <param name="type">Tipo esperado</param>
        /// <returns></returns>
        public static Type GetTypeValue(this GenericDbType type)
        {
            switch(type)
            {
                case GenericDbType.Integer:
                    return typeof(int);
                case GenericDbType.Float:
                    return typeof(float);
                case GenericDbType.Boolean:
                    return typeof(bool);
                case GenericDbType.Date:
                case GenericDbType.Time:
                case GenericDbType.DateTime:
                case GenericDbType.TimeStamp:
                    return typeof(DateTime);
                case GenericDbType.Bit:
                case GenericDbType.Byte:
                    return typeof(byte);
                case GenericDbType.Object:
                    return typeof(object);
                case GenericDbType.String:
                case GenericDbType.Unknown:
                default:
                    return typeof(string);
            }
        }
    }
}
